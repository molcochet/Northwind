FROM microsoft/dotnet:latest
COPY src /app
WORKDIR /app
RUN ["dotnet", "restore"]
WORKDIR /app/Northwind
RUN ["dotnet", "build"]
EXPOSE 5001/tcp
ENV ASPNETCORE_URLS https://*:5001
ENTRYPOINT ["dotnet", "run", "--server.urls", "http://*:5001"]