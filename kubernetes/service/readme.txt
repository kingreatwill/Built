kubectl get pods -n kube-system -o wide
kubectl get pods -o wide
kubectl get svc -o wide

kubectl create -f nginx-service.yaml