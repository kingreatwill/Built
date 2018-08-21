kubectl create -f nginx-deployment.yaml

kubectl get deployments
kubectl get rs
kubectl get pods --show-labels

升级方法1
kubectl set image deployment/nginx-deployment nginx=nginx:1.9.1
升级方法2 直接编辑
kubectl edit deployment/nginx-deployment

kubectl describe deployments