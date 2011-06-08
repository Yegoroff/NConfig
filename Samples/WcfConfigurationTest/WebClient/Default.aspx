<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebClient.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head id="Head1" runat="server">
   
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />

    <title>NConfig WCF Configuration Test.</title>
</head>
<body>
    <form id="Form1" runat="server">
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
            <p>
                WCF service endpoint address: <span id="endpointAddress" runat="server"></span>
            </p>

            <a href="Nest/nested.aspx">Nested Page</a>
        </div>
    </div>
    </form>
</body>
</html>



