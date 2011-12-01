<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Nested.aspx.cs" Inherits="WebClient.Nest.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <title>Nested Configuration Test</title>
</head>
<body>
    <form id="form1" runat="server">
    <div class="page">
        <div class="header">
            <div class="title">
                <h1>
                    ASP.NET NConfig.
                </h1>
            </div>
        </div>
        <div class="main">
            <h2>
                 WCF Configuration Test.
            </h2>

            <h3>NConfig Default TestConfigSection value: </h3>
            <p id="NConfigDefault" runat="server"></p>
            
            <h3>ConfigurationManager TestConfigSection value: </h3>
            <p id="ConfigManager" runat="server"></p>
            
            <h3>NConfig NamedSection value : </h3>
            <p id="NConfigNamed" runat="server"></p>

            <h3>Merged App Settings : </h3>
            <div id="AppSettings" runat="server" />

            <h3>Merged Connection Strings: </h3>
            <div id="ConnectionStrings"  runat="server" />

        </div>
    </div>
    </form>
</body>
</html>
