<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="ReportIssue.aspx.cs" Inherits="GadgetFox.ReportIssue" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="height: 0px; margin-top: 0px; font-style: normal; font-size: x-large; top: auto;">
         Report Issue
    </p>

    <div><asp:Label ID="returnLabel" runat="server" ForeColor="#CC0000"></asp:Label></div>
    <div>
        <asp:Panel ID="Panel1" runat="server" BorderColor="#999999" Height="390px" style="margin-left: 250px" Width="479px" Direction="LeftToRight" HorizontalAlign="Left">
            <table>
                <tr>
                    <td>Customer ID <asp:Label ID="Label0" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td><asp:TextBox ID="emailId" runat="server" style="margin-left: 2px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Issue ID <asp:Label ID="Label1" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td><asp:TextBox ID="issueId" runat="server" style="margin-left: 2px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Issue type <asp:Label ID="Label2" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:DropDownList ID="issueType" runat="server" style="margin-left: 2px">
                            <asp:ListItem Selected="True">-- Select --</asp:ListItem>
                            <asp:ListItem>Product defect</asp:ListItem>
                            <asp:ListItem>Product price</asp:ListItem>
                            <asp:ListItem>Product quality</asp:ListItem>
                            <asp:ListItem>Shipping</asp:ListItem>
                            <asp:ListItem>Other</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>Order ID <asp:Label ID="Label3" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td><asp:TextBox ID="orderId" runat="server" style="margin-left: 2px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Description <asp:Label ID="Label4" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td><asp:TextBox ID="issueDescription" runat="server" style="margin-left: 2px" Height="190px" Width="273px" TextMode="MultiLine"></asp:TextBox></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="saveButton" runat="server" Text="Submit" Width="75px" OnClick="saveButton_Clicked" style="margin-top: 20px"/>
                    </td>
                    <td>
                        <asp:Button ID="cancelButton" runat="server" Text="Cancel" Width="75px" OnClick="cancelButton_Clicked" PostBackUrl="~/Home.aspx" style="margin-top: 20px"/>
                    </td>
                </tr>
            </table>
        
        </asp:Panel>
    </div>
    </asp:Content>
