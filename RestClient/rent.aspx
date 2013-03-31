<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rent.aspx.cs" Inherits="RentIt.rent" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
                    <li><a href="/gspm2013/<%= RentIt.Utility.GetTeamPath(Server.MapPath("~/")) %>">Home</a></li>
                    <li class="active"><a href="#">Rent</a></li>
                    <li><a href="about.aspx">About Us</a></li>
                </ul>
            </div>
        </div>

        <form id="helloForm" runat="server">
            <div>
                <div class="row-fluid">
                    <asp:Table ID="Table1" runat="server" class="table table-bordered"></asp:Table>
                </div>
            </div>
        </form>

       
    </div>

</body>
</html>
