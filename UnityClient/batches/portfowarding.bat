echo portfowarding start

netsh interface portproxy add v4tov4 listenport=130 listenaddress=192.168.1.116 connectport=223 connectaddress=192.168.1.203

echo portfowarding finished
echo if you try any input, the port will be release
pause

netsh interface portproxy delete v4tov4 listenport= listenaddress= 

