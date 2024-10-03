# dotnet4k8s
.Net examples for Kubernetes

## Build containers

### Prerequisites
1. Windows
2. [Install WSL](https://learn.microsoft.com/en-us/windows/wsl/install) with Ubuntu-22.04
3. [Install Docker in WSL](https://docs.docker.com/engine/install/ubuntu/)
4. Apply fixes for Docker Engine in WSL ("Cannot connect to the Docker daemon at unix:///var/run/docker.sock" issue):
```
$ sudo update-alternatives --config iptables
Enter 1 to select iptables-legacy
```
5. Start Docker Engine
```
$ sudo service docker start
```
6. Install [dive](https://github.com/wagoodman/dive) to examine container images

### Build and test cli container
```
$ wsl
$ cd cli
$ docker build -f ./docker/Dockerfile.release -t test-cli:release .
$ docker run test-cli:release
Environment.OSVersion: 'Unix 5.10.102.1'
CWD: '/app'
This is message
$ docker run -e CONTAINERIZED_APP_MESSAGE='This is env message' test-cli:release
Environment.OSVersion: 'Unix 5.10.102.1'
CWD: '/app'
This is env message
```

### Build and test svc container
```
$ cd svc
$ docker build -f ./docker/Dockerfile.release -t test-svc:release .
$ docker run -d -p 8080:8080 test-svc:release
$ curl http://localhost:8080/WeatherForecast
[{"date":"2024-10-03","temperatureC":-3,"temperatureF":27,"summary":"Hot"},{"date":"2024-10-04","temperatureC":40,"temperatureF":103,"summary":"Chilly"},{"date":"2024-10-05","temperatureC":46,"temperatureF":114,"summary":"Freezing"},{"date":"2024-10-06","temperatureC":1,"temperatureF":33,"summary":"Hot"},{"date":"2024-10-07","temperatureC":-18,"temperatureF":0,"summary":"Chilly"}]
```

## Deploy to minikube

### Prerequisites
1. Install kubectl in WSL:
```
# Download the latest Kubectl
$ curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"

# Make it executable
$ chmod +x ./kubectl

# Move it to your user's executable PATH
$ sudo mv ./kubectl /usr/local/bin/
```

2. Install minikube in WSL:
```
$ curl -Lo minikube https://storage.googleapis.com/minikube/releases/latest/minikube-linux-amd64

# Make it executable
$ chmod +x ./minikube

# Move it to your user's executable PATH
$ sudo mv ./minikube /usr/local/bin/

#Set the driver version to Docker
$ minikube config set driver docker
```
3. Install ingress for minikube
```
$ minikube addons enable ingress
```

4. Start minikube:
```
$ minikube start --force
```

5. Copy minikube config:
```
$ kubectl config view --flatten
```
6. [Install OpenLens locally](https://github.com/MuhammedKalkan/OpenLens)
7. Add minikube in OpenLens: add new cluster from kubeconfig
8. Install [Helm](https://helm.sh/docs/intro/install/) in WSL


## Deploy cli helm to minikube
```
$ eval $(minikube docker-env)
$ cd cli
$ docker build -f ./docker/Dockerfile.release -t test-cli:release .
$ minikube image ls
$ export NAMESPACE=test
$ export CONTAINERIZED_APP_MESSAGE="This is message from the k8s configmap"
$ helm upgrade test-cli ./helm --install --create-namespace -n $NAMESPACE \
  --set backend.env.CONTAINERIZED_APP_MESSAGE="$CONTAINERIZED_APP_MESSAGE"
```

## Deploy svc helm to minikube
```
$ eval $(minikube docker-env)
$ cd svc
$ docker build -f ./docker/Dockerfile.release -t test-svc:release .
$ minikube image ls
$ export NAMESPACE=test
$ export CONTAINERIZED_APP_MESSAGE="This is message from the k8s configmap"
$ helm upgrade test-svc ./helm --install --create-namespace -n $NAMESPACE \
  --set backend.env.CONTAINERIZED_APP_MESSAGE="$CONTAINERIZED_APP_MESSAGE"
$ curl --resolve api.project.local:80:$(minikube ip) http://api.project.local/WeatherForecast
[{"date":"2024-10-03","temperatureC":37,"temperatureF":98,"summary":"Hot"},{"date":"2024-10-04","temperatureC":-17,"temperatureF":2,"summary":"Warm"},{"date":"2024-10-05","temperatureC":5,"temperatureF":40,"summary":"Scorching"},{"date":"2024-10-06","temperatureC":-8,"temperatureF":18,"summary":"Scorching"},{"date":"2024-10-07","temperatureC":-18,"temperatureF":0,"summary":"Warm"}]
# to call outside minikube read https://github.com/kubernetes/minikube/issues/19144
```
