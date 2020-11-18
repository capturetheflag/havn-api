# havn-api
RESTful API sample endpoint

### Docker instructions
1. Make sure that Docker is installed on your machine
2. Navigate to the app's root folder in your favourite terminal
3. Build the Docker image (mind the dot at the end) `docker build -t havn-api .`
4. Run the image `docker run -p 5000:80 havn-api`
5. Navigate in the browser to `http://localhost:5000/swagger`
