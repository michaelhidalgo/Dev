var virtualPath = '/../../Web Applications/TM_Website';
var webRoot = __dirname + virtualPath;

var express = require('express');
var app = express();

app.configure(function()
{
    app.use(express.directory(webRoot));
    app.use(express.static(webRoot));
});
app.listen(3005);
console.log('Listening on port 3005');
console.log('Web Root is at: ' + webRoot);
