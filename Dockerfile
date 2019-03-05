 FROM microsoft/dotnet:aspnetcore-runtime

 WORKDIR /app

 COPY ./src/TimeTeller/bin/Release/netcoreapp2.2/publish/ /app

 ENTRYPOINT [ "dotnet", "TimeTeller.dll" ]
