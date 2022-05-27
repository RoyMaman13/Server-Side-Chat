# Instruction for the entire project
 for this program you need to clone two repositories, the first one is this repositry (Chat-App-Server), and the second repository is under this account
 under the name Chat-App-Client, here's a link https://github.com/YONATAN-LAHAV/Chat-App-Client .
 
 ## Chat-App-Server instructions
 after cloning Chat-App-Server repository, do the following :
 1) open the solution - ChatWebServer.sln on Visual Studio.
 2) set the Web-server as the strart-up project.
 3) in the Package Manager Console type - update-database initialization
 4) set the web-api as the start up project and run it

## Chat-App-Client instructions
after cloning Chat-App-Client repository, do the following :
1) open the terminal in the cloned folder
2) type npm install (that may take a while)
3) type npm start

##

now you have the client side which shows the chat and the server side which shows the api funcitionalty on the swagger interface running on your local browser.
##

## Pay attention to the following :

1. the api-server is runing on localhost:7182 and can communicate with localhost:3000, and localhost:3001 only
2. This program is using database, therefore the data is localy saved on the users computer, that is why the chat will be empty for any data, (users, messages, conversation...), in your first run. So for using the chat properly register a few users, log in to them and add as many users, that you created, as you want to, send messages, and of course use the entire chat funcitionalty.
3. we also created a Rate page where any person can rate our page. to see this page stop the web-api run, setin  visual studio the Web-Server as the start up project and run it. the rate page will apper and will give you the full premmision to add edit delete and search rates.
