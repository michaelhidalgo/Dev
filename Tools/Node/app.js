var virtualPath = '/../../Web Applications/TM_Website';
    port = 3005,
    express = require('express'),
    path = require('path'),
    fs = require('fs'),
    app = express(),
    webRoot = path.join(__dirname, virtualPath),
    tmCached = path.join(__dirname, "TM_Cached");


var showFromCache = function(req, res, cacheName)
    {
        var file = path.join(tmCached,cacheName);
        fs.readFile( file, 'utf8', function (err,data)
                {
                    if (err)
                    {
                        res.end("couldn't find file: " + file);
                    }
                    res.end(data);
                });
    };

var getTime                      = function(req, res) {  showFromCache(req,res,"GetTime");                      };
var getGuiObjects                = function(req, res) {  showFromCache(req,res,"GetGuiObjects");                };
var getFolderStructure_Libraries = function(req, res) {  showFromCache(req,res,"GetFolderStructure_Libraries"); };
var currentUser                  = function(req, res) {  showFromCache(req,res,"CurrentUser");                  };
var rbac_CurrentPrincipal_Roles  = function(req, res) {  showFromCache(req,res,"RBAC_CurrentPrincipal_Roles");  };
var jsTreeWithFolders            = function(req, res) {  showFromCache(req,res,"JsTreeWithFolders");            };
var getGuidanceItemHtml          = function(req, res) {  showFromCache(req,res,"GetGuidanceItemHtml");          };
var searchTitleAndHtml           = function(req, res) {  showFromCache(req,res,"SearchTitleAndHtml");           };
var homePage                     = function(req,res)  {  res.redirect('/Html_Pages/Gui/TeamMentor.html');       };
var homePageCss                  = function(req, res)
                                        {
                                            switch(req.url.toString())
                                            {
                                                case "/aspx_Pages/scriptCombiner.ashx?s=HomePage_CSS&ct=css":
                                                        res.set('Content-Type', 'text/css');
                                                        showFromCache(req,res,"HomePage_CSS");
                                                        break;
                                                case "/aspx_Pages/scriptCombiner.ashx?s=HomePage_JS":
                                                        res.set('Content-Type', 'application/x-javascript');
                                                        showFromCache(req,res,"HomePage_JS");
                                                        break;
                                                case "/aspx_Pages/scriptCombiner.ashx?s=HomePage_JS_TM&dontMinify=true&v1":
                                                        res.set('Content-Type', 'application/x-javascript');
                                                        showFromCache(req,res,"HomePage_JS_TM");
                                                        break;
                                                //default:
                                                //        res.end("Unrecognizable: "+  req.url );
                                                //        break;

                                            }
                                        };

var configure = function()
    {
        app.get ('/'            , homePage);
        app.get ('/TeamMentor'  , homePage);
        app.post('/Aspx_Pages/TM_WebServices.asmx/GetTime'                                      , getTime);
        app.post('/Aspx_Pages/TM_WebServices.asmx/GetGuiObjects'                                , getGuiObjects);
        app.post('/Aspx_Pages/TM_WebServices.asmx/GetFolderStructure_Libraries'                 , getFolderStructure_Libraries);
        app.post('/Aspx_Pages/TM_WebServices.asmx/Current_User'                                 , currentUser);
        app.post('/Aspx_Pages/TM_WebServices.asmx/RBAC_CurrentPrincipal_Roles'                  , rbac_CurrentPrincipal_Roles);
        app.post('/Aspx_Pages/TM_WebServices.asmx/JsTreeWithFolders'                            , jsTreeWithFolders);
        app.post('/Aspx_Pages/TM_WebServices.asmx/GetGuidanceItemHtml'                          , getGuidanceItemHtml);
        app.post('/Aspx_Pages/TM_WebServices.asmx/XmlDatabase_GuidanceItems_SearchTitleAndHtml' ,  searchTitleAndHtml);
        app.get ('/Aspx_Pages/scriptCombiner.ashx'                                              , homePageCss);

        app.use (express.static(webRoot));
        app.use (express.directory(webRoot));
    };

app.configure(configure)
   .listen(3005);

console.log('Listening on port ' + port);
console.log('Web Root is at: ' + webRoot);
