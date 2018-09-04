1、 查看配置文件位于哪里
systemctl show --property=FragmentPath docker 
2、编辑配置文件内容，接收所有ip请求
vim  /lib/systemd/system/docker.service 
ExecStart=/usr/bin/dockerd -H unix:///var/run/docker.sock -H tcp://0.0.0.0:4243
3、重新加载配置文件，重启docker daemon
sudo systemctl daemon-reload 
sudo systemctl restart docker
4、测试
# docker -H localhost:4243 version