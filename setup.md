# Setup Kubernetes 3-tier system
## Table of content
- [Setup Kubernetes 3-tier system](#setup-kubernetes-3-tier-system)
  - [Table of content](#table-of-content)
  - [Setup Azure Kubernetes](#setup-azure-kubernetes)
    - [Create a container Registry (ACR)](#create-a-container-registry-acr)
    - [Create an AKS Cluster](#create-an-aks-cluster)
    - [Install kubernetes cli (kubectl)](#install-kubernetes-cli-kubectl)
    - [Install kubernetes dashboard (Optional)](#install-kubernetes-dashboard-optional)
  - [Setup SqlServer](#setup-sqlserver)
    - [Create a Persistent Disk (Dynamic)](#create-a-persistent-disk-dynamic)
    - [Create a SqlServer password](#create-a-sqlserver-password)
    - [Deploy SqlServer](#deploy-sqlserver)
  - [Setup Backend WepApi](#setup-backend-wepapi)
    - [Using public IP address](#using-public-ip-address)
      - [Using database connectionstring in appsettings.json](#using-database-connectionstring-in-appsettingsjson)
    - [Using a Private IP Address for the database (ClusterIP)](#using-a-private-ip-address-for-the-database-clusterip)
  - [Setup the frontend](#setup-the-frontend)
  - [Setup Ingress](#setup-ingress)
    - [Without certificate](#without-certificate)
    - [With certificate](#with-certificate)
  - [Horizontal Pod Autoscaler (HPA)](#horizontal-pod-autoscaler-hpa)
## Setup Azure Kubernetes
To setup AKS you need to create a container registry first
### Create a container Registry (ACR)
Can be done in the portal, but with cli as well.
```shell 
az acr create -g myResourceGroup -n mycontainerregistry --sku Basic
```
### Create an AKS Cluster
This can be done in the portal. With cli it's a bit tricky
```shell
az aks create -g myResourceGroup -n myAKSCluster --node-count 1 --attach-acr acrName --tier free
```
### Install kubernetes cli (kubectl)
Is needed to control kubernetes.
Url: https://kubernetes.io/docs/tasks/tools/install-kubectl-windows/  
Or via Azure cli.
```shell
az aks install-cli
```
Next you need to authenticate kubectl.
```shell  
az aks get-credentials -g myResourceGroup -n myAKSCluster
```
### Install kubernetes dashboard (Optional)
Optionally you can install the kubernetes dashboard for an overview in your cluster.
```shell
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml
```
The following two yaml fragments can be found in [dashboard-admin.yaml](dashboard-admin.yaml)  
upload the file by:
```shell
kubectl apply -f dashboard-admin.yaml
```

First create a Service Account
```yaml
apiVersion: v1
kind: ServiceAccount
metadata:
  name: admin-user
  namespace: kubernetes-dashboard
```
Finally create a clusterRoleBinding
```yaml
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: admin-user
roleRef:
  apiGroup: rbac.authorization.k8s.io
  kind: ClusterRole
  name: cluster-admin
subjects:
- kind: ServiceAccount
  name: admin-user
  namespace: kubernetes-dashboard
```
Now we need to find the token we can use to log in. Execute the following command:

```shell
kubectl -n kubernetes-dashboard create token admin-user
```
And copy the resulting token. You need is to signin the dashboard.  
Now start the local service
```shell
kubectl proxy
```
and navigate to http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/

## Setup SqlServer
### Create a Persistent Disk (Dynamic)
Data in containers will be gone once the container is destroyed.  
Therefore we create persistent volumes. Check out the supported types (Storage Classes, sc)
You can create your own storage class like this one: [storage-class.yaml](storage-class.yaml)
```shell
kubectl get sc
```
Next post the following [yaml file](persistent-volume.yaml)  
where accessModes ReadWriteOnce means the the drive can be used by only one node
```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
    name: azure-managed-disk
spec:
  accessModes:
  - ReadWriteOnce
  storageClassName: managed-csi
  resources:
    requests:
      storage: 5Gi
```
And then:
```shell
kubectl apply -f persistent-volume.yaml
```
### Create a SqlServer password
secrets are stored in the Kubernetes secrets store (vault)
```shell
kubectl create secret generic mssql --from-literal=MSSQL_SA_PASSWORD="Test_1234567"
```
### Deploy SqlServer
The SqlServer deployment is define in this yaml file [sqlserver-deployment.yaml](sqlserver-deployment.yaml)
Deploy it by using  
```shell
kubectl apply -f sqlserver-deployment.yaml
```
To make it accessibel from the outside add a load balancer [sqlserver-loadbalancer.yaml](sqlserver-loadbalancer.yaml)
```yaml
apiVersion: v1
kind: Service
metadata:
  name: mssql-deployment
spec:
  selector:
    app: mssql
  ports:
    - protocol: TCP
      port: 1433
      targetPort: 1433
  type: LoadBalancer
  ```
  Deploy the Loadbalancer
  ```shell
  kubectl apply -f sqlserver-loadbalancer.yaml
```
## Setup Backend WepApi
### Using public IP address
In this scenarion we deploy the WepApi project using the [Kubernetes LoadBalancer Service](#deploy-sqlserver).
#### Using database connectionstring in appsettings.json
ConnectionStrings are usually defined in the appsettings,json which can be read from your code. 
In your WebApi project read from the configuration the connection string
```csharp
IConfiguration config = builder.Configuration;
var connectionString = config.GetConnectionString("SqlServer");
```
How do you get this appsettings.json safely in Kubernetes without revealing the connectionstring?
Define a [ConfigMap](config-map-backend.yaml) in Kubernetes
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: apisettings
data:
  appsettings.json: |-
    {
      "ConnectionStrings": {
        "SqlServer": "Server=<ip address>;Database=ProductCatalog;User Id=sa;Password=<big secret>;"
      }
    }
```
and 
```shell
kubectl apply -f config-map-backend.yaml
```
Next create a Deployment for your [Api Project](Application/Yaml/ACME.Backend.yaml)
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: shopapi-deployment
  labels:
    app: shopapi
    tier: backend
spec:
  replicas: 1
  selector:
    matchLabels:
      app: shopapi
      tier: backend
  template:
    metadata:
      labels:
        app: shopapi
        tier: backend
    spec:
      containers:
      - name: shopapi
        image: psimages.azurecr.io/shopapi:v5
        ports:
        - containerPort: 80
        env:
        - name: "ASPNETCORE_ENVIRONMENT"
          value: "Kubernetes"
        volumeMounts:
        - name: appsettings-volume
          mountPath: /app/appsettings.json
          subPath: appsettings.json
      volumes:
      - name: appsettings-volume
        configMap: 
          name: apisettings
```
Note the last part with the volumes.
We create a volume containing the data in our ConfigMap and mount is in our container (volumeMounts)
```yaml
      containers:
        ...
        volumeMounts:
        - name: appsettings-volume
          mountPath: /app/appsettings.json
          subPath: appsettings.json
      volumes:
      - name: appsettings-volume
        configMap: 
          name: apisettings
```
Finally we need a LoadBalancer to make the service publicly available
```yaml
apiVersion: v1
kind: Service
metadata:
  name: shopapi 
spec:
  type: LoadBalancer
  selector:
    app: shopapi
    tier: backend
  ports:
  - name: http
    port: 80

```
### Using a Private IP Address for the database (ClusterIP)
It's better to keep the address of SqlServer private. For that we create a [ClusterIP Service](sqlserver-clusterip.yaml)
```yaml
apiVersion: v1
kind: Service
metadata:
  name: mssql-ep
spec:
  type: ClusterIP
  selector:
    app: mssql
  ports:
    - protocol: TCP
      port: 1433
      targetPort: 1433
```
Now change the connectionstring in your [ConfigMap](config-map-backend.yaml) to
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: apisettings
data:
  appsettings.json: |-
    {
      "ConnectionStrings": {
        "SqlServer": "Server=mssql-ep;Database=ProductCatalog;User Id=sa;Password=<big secret>;"
      }
    }
```
Note that we use the [ClusterIP](sqlserver-clusterip.yaml) name here instead of a hardcoded ip address.
## Setup the frontend
TODO
## Setup Ingress
### Without certificate
Using a LoadBalancer Service to expose your service is pretty straight forward but those ip-addresses are not very user-friendly. Also https is awkward. An alternative is using ingress.
An Ingress is nothing more than a set of routing rules. They do nothing. You need a special Deployment: An Ingress Controller. The Ingress Controller processes all the rules defined in the Ingress. Now there are many Ingress Controllers in the market and many are commercial. In this case we use the nginx controller (https://kubernetes.github.io/ingress-nginx/)
Installing the nginx controller just execute the following command:
```shell
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.0/deploy/static/provider/cloud/deploy.yaml
```
To check if the controller is running execute the following command:
```shell
kubectl get pods --namespace=ingress-nginx
```
Now it's time to define rules. See [Ingress](ingress-frontend.yaml)
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: ingress-frontend
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /
spec:
  ingressClassName: nginx
  rules:
    - http:
        paths:
        - path: /
          pathType: Prefix
          backend:
            service:
              name: shopmvc-ep
              port:
                number: 80
```
### With certificate
Although the previous configuration works, it's better to add a certificate.
In this case we'll use a self-signed certificate.
First create the certificate
```shell
dotnet dev-certs https -ep ./Certs/ingress.pem --format PEM -np
```
Next create a secret in Kubernetes
```shell
kubectl create secret tls ingress-cert --key=Certs\ingress.key --cert=Certs\ingress.pem -o yaml
```
And finally add the secret to the [Ingress](ingress-tls-frontend.yaml)
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: ingress-frontend
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /
spec:
  ingressClassName: nginx
  tls:
  - hosts:
    - pstrainingen.nl
    secretName: ingress-cert
  rules:
  - host: pstrainingen.nl
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: shopmvc-ep
            port:
              number: 80
```
The nginx controller comes with a default certificate (ACME). To surpress that one add the following startup argument to the nginx controller
```yaml
--default-ssl-certificate=default/ingress-cert
```
To add the above statement edit the nginx-controller under the ingress-nginx namespace
```yaml
...
containers:
- name: controller
  image: >-
    registry.k8s.io/ingress-nginx/controller:v1.8.0@sha256:744ae2afd433a395eeb13dc03d3313facba92e96ad71d9feaafc85925493fee3
  args:
    - /nginx-ingress-controller
    - '--publish-service=$(POD_NAMESPACE)/ingress-nginx-controller'
    - '--election-id=ingress-nginx-leader'
    - '--controller-class=k8s.io/ingress-nginx'
    - '--ingress-class=nginx'
    - '--configmap=$(POD_NAMESPACE)/ingress-nginx-controller'
    - '--validating-webhook=:8443'
    - '--validating-webhook-certificate=/usr/local/certificates/cert'
    - '--validating-webhook-key=/usr/local/certificates/key'
    - '--default-ssl-certificate=default/ingress-cert'  # Add this line!
  ports:
    - name: http
      containerPort: 80
      protocol: TCP
...
```
## Horizontal Pod Autoscaler (HPA)
When de app gets more popular you probably want to scale out but probably not permanently.
Welcome to the Horizontal Pod Autoscvaler (HPA).
Here you can define criteria on which your applications scales out or in.
You kan activate HPA on the kubectl command prompt:
```shell
kubectl autoscale deployment shopmvc-deployment --cpu-percent=50 --min=1 --max=10
```
However in a yaml file it might be more convenient.
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: hpa-shopmvc-deployment
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: shopmvc-deployment
  minReplicas: 1
  maxReplicas: 5
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 30
```

Test 
```powershell
for ($i=0; $i -le 10; $i++) {Invoke-WebRequest -Uri https://pstrainingen.nl -SkipCertificateCheck}
```




