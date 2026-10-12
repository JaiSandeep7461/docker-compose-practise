# Docker Compose Demo: Frontend + .NET Backend

A beginner-friendly, hands-on demonstration of how to build Docker
images and run containers individually, then use Docker Compose to
manage the same application.

## 1. What this project demonstrates

-   Build the frontend image with Docker.
-   Build the .NET backend image with Docker.
-   Run and test each container independently.
-   Remove the standalone containers.
-   Use Docker Compose to build and start both services together.
-   Check service status, view logs, test endpoints, and clean up.

### Architecture

``` text
Browser / curl
   |
   | http://localhost:8082
   v
Frontend container (Nginx, port 80)

Backend API:
http://localhost:8081/api
   |
   v
.NET backend container (ASP.NET Core, port 8080)
```

The frontend is a simple HTML page served by Nginx. The backend is a
minimal .NET Web API. In this demo, the frontend page and API are tested
separately; the HTML page does not call the API.

## 2. Project structure

``` text
docker-compose-demo/
├── frontend/
│   ├── src/
│   │   └── index.html
│   └── Dockerfile
├── backend/
│   ├── Program.cs
│   └── Dockerfile
└── compose.yaml
```

## 3. Prerequisites

-   Docker Desktop is installed and running.
-   Run the commands from the `docker-compose-demo` project directory.
-   The .NET SDK does not need to be installed on the host. Docker
    downloads the .NET SDK and runtime images during the backend build.

Check Docker:

``` bash
docker --version
docker compose version
docker info
```

## Part A --- Build and run containers using Docker commands

We will build and test each image separately before using Compose.

### 4. Build the backend image

``` bash
docker build -t demo-backend:1.0 ./backend
```

Explanation: - `docker build` builds an image from a Dockerfile. -
`-t demo-backend:1.0` gives the image a name and tag. - `./backend` is
the build context; Docker finds `backend/Dockerfile` there.

Verify the image:

``` bash
docker images demo-backend
```

### 5. Run and test the backend container

``` bash
docker run -d --name demo-backend -p 8081:8080 demo-backend:1.0
```

Explanation: - `-d` runs the container in the background. -
`--name demo-backend` assigns a readable container name. -
`-p 8081:8080` maps host port `8081` to container port `8080`. -
`demo-backend:1.0` is the image used to create the container.

Check status and logs:

``` bash
docker ps
docker logs demo-backend
```

Test the API:

``` bash
curl http://localhost:8081/api
```

Expected response:

``` json
{"message":"Hello from .NET Backend API!"}
```

### 6. Build the frontend image

``` bash
docker build -t demo-frontend:1.0 ./frontend
```

Explanation: - `-t demo-frontend:1.0` names the frontend image. -
`./frontend` contains the frontend Dockerfile and HTML file.

Verify the image:

``` bash
docker images demo-frontend
```

### 7. Run and test the frontend container

``` bash
docker run -d --name demo-frontend -p 8082:80 demo-frontend:1.0
```

Explanation: - `-d` runs the container in the background. -
`--name demo-frontend` assigns its container name. - `-p 8082:80` maps
host port `8082` to Nginx's container port `80`.

Check status and test:

``` bash
docker ps
curl http://localhost:8082
```

Open the frontend in a browser:

-   <http://localhost:8082>

The page should display **Docker Compose Demo** and the frontend success
message.

### 8. Remove the standalone containers

We have now tested each image independently. Remove the containers
before starting the Compose version:

``` bash
docker rm -f demo-backend
docker rm -f demo-frontend
```

Explanation: - `docker rm` removes a container. - `-f` stops the
container if necessary and then removes it. - These commands do **not**
delete the images.

Confirm that the standalone containers have been removed:

``` bash
docker ps -a
```

The `demo-backend` and `demo-frontend` containers should no longer
appear. Other unrelated containers may still be listed.

## Part B --- Build and run the application using Docker Compose

### 9. Review `compose.yaml`

The file should contain:

``` yaml
services:
  frontend:
    build: ./frontend
    ports:
      - "8082:80"

  backend:
    build: ./backend
    ports:
      - "8081:8080"
```

Explanation: - `services` lists the services that make up this
application. - `frontend` and `backend` are Compose service names. -
`build: ./frontend` tells Compose to build an image using
`frontend/Dockerfile`. - `build: ./backend` tells Compose to build an
image using `backend/Dockerfile`. - `ports` publishes container ports to
the host so we can test them from Windows.

Compose automatically creates a project network for these services. This
example does not require `depends_on`, because the frontend does not
make requests to the backend.

Validate the Compose configuration:

``` bash
docker compose config
```

This displays the resolved configuration and helps catch YAML or
configuration errors.

### 10. Build and start both services

``` bash
docker compose up --build -d
```

Explanation: - `up` creates and starts the services. - `--build` asks
Compose to build the images before starting. - `-d` runs the containers
in the background.

Compose builds from the Dockerfiles, creates the service containers,
sets up the project network, and publishes the configured ports.

### 11. Verify the Compose services

``` bash
docker compose ps
```

Both `frontend` and `backend` should show a running status.

View logs:

``` bash
docker compose logs
```

View logs for one service:

``` bash
docker compose logs frontend
docker compose logs backend
```

Follow logs continuously:

``` bash
docker compose logs -f
```

Press `Ctrl+C` to stop following logs. This does not stop the
containers.

### 12. Test both services

Test the frontend:

``` bash
curl http://localhost:8082
```

Or open <http://localhost:8082> in a browser.

Test the backend:

``` bash
curl http://localhost:8081/api
```

Expected backend response:

``` json
{"message":"Hello from .NET Backend API!"}
```

## 13. Useful Docker Compose commands

  -----------------------------------------------------------------------
  Command                             What it does
  ----------------------------------- -----------------------------------
  `docker compose config`             Validates and displays the resolved
                                      configuration.

  `docker compose build`              Builds service images without
                                      starting the services.

  `docker compose up --build -d`      Builds images and creates/starts
                                      services in the background.

  `docker compose up -d`              Creates/starts services without
                                      explicitly requesting a build.
                                      Compose may recreate services if it
                                      detects relevant changes.

  `docker compose ps`                 Shows the project's service
                                      containers and their status.

  `docker compose logs`               Displays service logs.

  `docker compose logs -f`            Follows logs as new output arrives.

  `docker compose stop`               Stops services but retains their
                                      containers.

  `docker compose start`              Starts existing stopped service
                                      containers.

  `docker compose restart`            Restarts service containers.

  `docker compose down`               Stops and removes the project's
                                      containers and default network.
                                      Images are retained by default.
  -----------------------------------------------------------------------

## 14. Stop and clean up the Compose application

When the demonstration is complete, run:

``` bash
docker compose down
```

This removes the Compose containers and the default project network. It
does not normally remove the built images.

Check the remaining images:

``` bash
docker images
```

If you want to run the demo again, use:

``` bash
docker compose up --build -d
```

## 15. Docker vs. Docker Compose

  -----------------------------------------------------------------------
  Docker commands                     Docker Compose
  ----------------------------------- -----------------------------------
  Build an individual image with      Defines image builds for multiple
  `docker build`.                     services in `compose.yaml`.

  Start a container with              Creates and starts the declared
  `docker run`.                       services with `docker compose up`.

  Ports and container settings are    Service settings are kept in a YAML
  supplied in commands.               file.

  Manage containers individually.     Manage the related application
                                      services together.
  -----------------------------------------------------------------------

**Key takeaway:** Docker builds images and runs containers. Docker
Compose uses a YAML file to define and manage multiple related services
with a consistent set of commands.

## 16. Demo sequence checklist

Use this order when presenting the project:

1.  Show the project structure.
2.  Build the backend image using `docker build`.
3.  Run and test the backend using `docker run`, `docker ps`,
    `docker logs`, and `curl`.
4.  Build the frontend image using `docker build`.
5.  Run and test the frontend in the browser or with `curl`.
6.  Remove both standalone containers using `docker rm -f`.
7.  Explain `compose.yaml`.
8.  Run `docker compose config`.
9.  Run `docker compose up --build -d`.
10. Verify with `docker compose ps` and `docker compose logs`.
11. Test both endpoints again.
12. Finish with `docker compose down`.

This sequence makes the purpose of Docker Compose clear: the services
can be built and run separately, but Compose lets us define and manage
them together.
# docker-compose-practise
