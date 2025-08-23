#
# Build stage
#
FROM maven:3.9.5-eclipse-temurin-21-alpine AS build
# Set the working directory inside the container
WORKDIR /app
# Copy the pom.xml and the src folder
COPY src /home/app/src
COPY pom.xml /home/app
# run maven clean and package without tests
RUN mvn -DskipTests -f /home/app/pom.xml clean package

#
# Package stage
#
FROM openjdk:21-jdk-slim
COPY --from=build /home/app/target/backend-java-*.jar /usr/local/lib/app.jar
# Expose the port your app runs on (default is 8080)
EXPOSE 8080
# Run the jar file
ENTRYPOINT ["java","-jar","/usr/local/lib/app.jar"]

