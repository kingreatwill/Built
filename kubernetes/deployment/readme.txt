kubectl create -f nginx-deployment.yaml

kubectl get deployments
kubectl get rs
kubectl get pods --show-labels

升级方法1
kubectl set image deployment/nginx-deployment nginx=nginx:1.9.1
升级方法2 直接编辑
kubectl edit deployment/nginx-deployment

kubectl describe deployments

查看pod日志，访问curl  查看负载情况
kubectl logs -f pod-name
kubectl describe pod pod-name
kubectl describe svc svc-name
==
kubectl describe svc/svc-name