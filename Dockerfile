FROM microsoft/dotnet
WORKDIR /app
EXPOSE 80
ADD O2O-Server/obj/Docker/publish /app
RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
ENTRYPOINT ["dotnet", "O2O-Server.dll"]
