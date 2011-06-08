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
            <p>
                Application Settings: <span id="applicationSettings" runat="server">NOTHING</span>
            </p>
            <p>
                Connection Strings: <span id="connectionStrings" runat="server">NOTHING</span>
            </p>

        </div>
    </div>
    </form>
</body>
</html>
