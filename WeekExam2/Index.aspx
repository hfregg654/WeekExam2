<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WeekExam2.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="Script/bootstrap.min.css" rel="stylesheet" />
    <script src="Script/jquery-3.6.0.min.js"></script>
    <script src="Script/bootstrap.min.js"></script>
    <style>
        .centertitle {
            text-align: center;
        }

        .centerinner {
            border: solid blue 2px;
            text-align: center;
        }

        .inner {
            background-color: gray;
            color: white;
            border: solid black 2px;
            text-align: center;
        }

        .togglediv {
            display: none;
        }

        .Nodata{
            font-size:120px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row centertitle">
                <div class="col-6">
                    <button type="button" id="GotoCommitbtn" class="">跳至留言區</button>
                </div>
                <div class="col-6">
                    <button type="button" id="ToggleCommit" class="">收合/展開回文</button>
                </div>
            </div>
        </div>
        <div class="container">
            <div class="row centertitle">
                <div id="Nothingdiv" class="col-12 Nodata" runat="server">
                    無留言
                </div>
                <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand" OnItemDataBound="Repeater1_ItemDataBound">
                    <ItemTemplate>
                        <div class="col-12 centerinner">
                            <div class="row">
                                <div class="col-2">
                                    <asp:Button ID="ButtonReCommit" runat="server" Text="回文" CommandName="CommitItem"
                                        CommandArgument='<%#Eval("CommitID") %>' />
                                </div>
                                <div class="col-4">#<%#Eval("CommitID") %> <%#Eval("UserName") %></div>
                                <div class="col-6"><%#Eval("CommitTime","{0:yyyy-MM-dd HH:mm:ss}") %></div>
                                <div class="col-10">
                                    <h5><%#Eval("Title") %></h5>
                                </div>
                                <div class="col-2">留言數(<asp:Literal ID="Literal2" runat="server"></asp:Literal>)</div>
                                <div class="col-12" style="text-align: center; padding-right: 400px;"><%#Eval("UserCommit") %></div>
                            </div>
                            <asp:Repeater ID="Repeater2" runat="server">
                                <ItemTemplate>
                                    <div class="col-12 inner">
                                        <div class="row">
                                            <div class="col-6">#<%#Eval("ReCommitID") %> <%#Eval("UserName") %></div>
                                            <div class="col-6"><%#Eval("CommitTime","{0:yyyy/MM/dd HH:mm:ss}") %></div>
                                            <div class="Togglediv col-12">
                                                <div class="col-12">
                                                    <h5><%#Eval("Title") %></h5>
                                                </div>
                                                <div class="col-12" style="text-align: center; padding-right: 35px"><%#Eval("UserCommit") %></div>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>




        <div class="container">
            <div class="row">
                <a href="./Index.aspx?Page=1" title="前往第1頁">First</a>｜
                <asp:Repeater runat="server" ID="repPaging" OnItemDataBound="repPaging_ItemDataBound">
                    <ItemTemplate>
                        <a href="<%# Eval("Link") %>" title="<%# Eval("Title") %>"><%# Eval("Name") %></a>｜
                    </ItemTemplate>
                </asp:Repeater>
                <asp:HyperLink ID="HyperLink1" runat="server" ToolTip="前往尾頁">Last</asp:HyperLink>
                <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                    <asp:ListItem Selected="True" Value="10">10</asp:ListItem>
                    <asp:ListItem Value="20">20</asp:ListItem>
                    <asp:ListItem Value="30">30</asp:ListItem>
                    <asp:ListItem Value="50">50</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>





        <div class="container" id="CommitArea" style="margin-bottom: 250px;">
            <div class="row centerinner">
                <div class="col-12">
                    暱稱：：<asp:TextBox ID="TextBox1" runat="server" MaxLength="30"></asp:TextBox>
                    <span style="color:red"><asp:Literal Text="text" runat="server" ID="ltlNickName" Visible="False" /></span>
                </div>
                <div class="col-12">
                    標題：<asp:TextBox ID="TextBox2" runat="server" MaxLength="100"></asp:TextBox>
                    <span style="color:red"><asp:Literal Text="text" runat="server" ID="ltlTitle" Visible="False" /></span>
                </div>
                <div class="col-12">
                    內文：<asp:TextBox ID="TextBox3" runat="server" TextMode="MultiLine" MaxLength="250"></asp:TextBox>
                    <span style="color:red"><asp:Literal Text="text" runat="server" ID="ltlCom" Visible="False" /></span>
                </div>
                <div id="Back">回覆#<asp:Label ID="Label1" runat="server" Text=""></asp:Label></div>
                <div class="col-12 container">
                    <div class="row">
                        <div class="col-6">
                            <asp:Button ID="Submit" runat="server" Text="OK" OnClick="Submit_Click" />
                        </div>
                        <div class="col-6">
                            <asp:Button ID="Cancel" runat="server" Text="取消" OnClick="Cancel_Click1" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>





<script> 
    $(document).ready(function () {
        $("#Back").hide();
        var a = document.getElementById("Label1");
        if (a.textContent != "0" && a.textContent != "") {
            window.location.hash = "#CommitArea";
            $("#Back").show();
        } else {
            $("#Back").hide();
        };
        $("#GotoCommitbtn").click(function () {
            window.location.hash = "#CommitArea";
        });
        var divlist = document.getElementsByClassName("Togglediv");
        for (var i = 0; i < divlist.length; i++) {
            divlist[i].classList.add('togglediv');
        }
        var tg = false;
        $("#ToggleCommit").click(function () {
            if (tg == true) {
                for (var i = 0; i < divlist.length; i++) {
                    divlist[i].classList.add('togglediv');
                }
                tg = false;
            } else {
                for (var i = 0; i < divlist.length; i++) {
                    divlist[i].classList.remove('togglediv');
                }
                tg = true;
            }
        });
        $(".inner").click(function () {
            var td = this.getElementsByClassName("Togglediv")[0];
            td.classList.toggle('togglediv');
            tg = true;
        });



    });

</script>
</html>
