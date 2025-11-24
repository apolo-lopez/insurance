FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ./*.slnx ./ 
COPY ./Evaluation.API/*.csproj ./src/Evaluation.API/
COPY ./Evaluation.Infrastructure/*.csproj ./src/Evaluation.Infrastructure/
COPY ./Evaluation.Application/*.csproj ./src/Evaluation.Application/
COPY ./Evaluation.Domain/*.csproj ./src/Evaluation.Domain/

RUN dotnet restore "./src/Evaluation.Application/Evaluation.Application.csproj"
RUN dotnet restore "./src/Evaluation.Infrastructure/Evaluation.Infrastructure.csproj"
RUN dotnet restore "./src/Evaluation.API/Evaluation.API.csproj"

# copy everything else and build
COPY ./ ./src
WORKDIR /src/src/Evaluation.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Evaluation.API.dll"]