docker run --name mongo0 -d -p 27117:27017 mongo:4.1.2 --replSet "rs0" --bind_ip_all
docker run --name mongo1 -d -p 27217:27017 mongo:4.1.2 --replSet "rs0" --bind_ip_all
docker run --name mongo2 -d -p 27317:27017 mongo:4.1.2 --replSet "rs0" --bind_ip_all

docker inspect mongo0 | grep IPAddress   --172.17.0.8
172.17.0.9
172.17.0.10

docker exec -it mongo0 /bin/bash

rs.initiate( {
   _id : "rs0",
   members: [
      { _id: 0, host: "172.17.0.8:27017" },
      { _id: 1, host: "172.17.0.9:27017" },
      { _id: 2, host: "172.17.0.10:27017" }
   ]
})
mongo mongodb://172.17.0.8:27017,172.17.0.9:27017,172.17.0.10:27017/test?replicaSet=rs0
mongo mongodb://192.168.1.230:27117,192.168.1.230:27217,192.168.1.230:27317/test?replicaSet=rs0
mongo mongodb://192.168.1.230:27017/test
mongo --host 172.17.0.8
// 允许当前连接对从库进行读操作
db.getMongo().setSlaveOk()
rs.slaveOk();
rs.slaveOk(true)
rs.status()
rs.config()

docker cp mongo0:/usr/bin/mongo  /usr/bin/mongo 