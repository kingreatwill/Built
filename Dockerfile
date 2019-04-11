FROM docker.io/microsoft/dotnet:2.1-aspnetcore-runtime
COPY . /app
WORKDIR /app    

# soft link
RUN ln -s /lib/x86_64-linux-gnu/libdl-2.24.so /lib/x86_64-linux-gnu/libdl.so

# configure apt sources, only for Chinese users
RUN mv /etc/apt/sources.list /etc/apt/sources.list.bak && \
   echo "deb http://mirrors.163.com/debian/ jessie main non-free contrib" >/etc/apt/sources.list && \
   echo "deb http://mirrors.163.com/debian/ jessie-proposed-updates main non-free contrib" >>/etc/apt/sources.list && \
   echo "deb-src http://mirrors.163.com/debian/ jessie main non-free contrib" >>/etc/apt/sources.list && \
   echo "deb-src http://mirrors.163.com/debian/ jessie-proposed-updates main non-free contrib" >>/etc/apt/sources.list

# install System.Drawing native dependencies
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
   		libgdiplus \
#         libc6-dev \
#         libgdiplus \
#         libx11-dev \
     && rm -rf /var/lib/apt/lists/*

CMD ["dotnet", "xxx.dll"]
