FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app/out
COPY ./out .

EXPOSE 80
ENTRYPOINT ["dotnet", "bkmarker.dll"]

