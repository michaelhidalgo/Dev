var virtualPath = '/../../Web Applications/TM_Website';
var webRoot = __dirname + virtualPath;

var express = express();
express.configure(function()
{
    this.use(express.directory(webRoot));
    this.use(express.static(webRoot));
})
    .listen(3005);

console.log('Listening on port 3005');
console.log('Web Root is at: ' + webRoot);
