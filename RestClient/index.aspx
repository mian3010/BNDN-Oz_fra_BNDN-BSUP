<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="RentIt.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <script src="http://code.jquery.com/jquery-latest.js"></script>
    <script src="js/bootstrap.min.js"></script>
</head>
<body>
    <div class="container">
        <div class="navbar navbar-inverse">
            <div class="navbar-inner">
                <a class="brand" href="/gspm2013/<%= RentIt.Utility.GetTeamPath(Server.MapPath("~/")) %>">RentIt</a>
                <ul class="nav">
                    <li class="active"><a href="#">Home</a></li>
                    <li><a href="rent.aspx">Rent</a></li>
                    <li><a href="about.aspx">About Us</a></li>
                </ul>
            </div>
        </div>
        <form id="helloForm" runat="server">
            <div>
                <div class="row-fluid">
                    <asp:Button ID="helloButton" runat="server" Text="Invoke" class="btn" OnClick="helloButton_Click" />
                    <asp:Label ID="helloLabel" runat="server" Text="<-- Click the button" class="label"></asp:Label>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
