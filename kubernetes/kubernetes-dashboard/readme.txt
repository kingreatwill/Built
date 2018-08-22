kubectl 命令来创建简单的 kubernetes-dashboard 服务：（Quick language switcher chrome插件可以切换中英文，或者设置将你想要的语言置顶到第一个就可以了，不用选择该语言）
1，
kubectl create -f https://raw.githubusercontent.com/kubernetes/dashboard/master/src/deploy/recommended/kubernetes-dashboard.yaml
查看
kubectl get deployments --namespace kube-system
在 Dashboard 启动完毕后，可以使用 kubectl 提供的 Proxy 服务来访问该面板：
kubectl proxy

# 打开如下地址：
# http://localhost:8001/api/v1/namespaces/kube-system/services/https:kubernetes-dashboard:/proxy/
如果访问报错，可以尝试编辑 kubernetes-dashboard 服务将ClusterIP 改为 NodePort（https://github.com/kubernetes/dashboard/wiki/Accessing-Dashboard---1.7.X-and-above）