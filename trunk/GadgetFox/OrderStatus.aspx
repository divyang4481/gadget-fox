<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="OrderStatus.aspx.cs" Inherits="GadgetFox.OrderStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="OrderID" DataSourceID="dsOrders" Width="70%">
            <Columns>
                <asp:BoundField DataField="OrderID" HeaderText="OrderID" ReadOnly="True" SortExpression="OrderID" />
                <asp:BoundField DataField="EmailID" HeaderText="EmailID" SortExpression="EmailID" ReadOnly="true" />
                <asp:TemplateField HeaderText="Status" SortExpression="Status">
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlStatus" runat="server" SelectedValue='<%# Bind("Status") %>'>
                            <asp:ListItem Value="-Select-"></asp:ListItem>
                            <asp:ListItem>Processing</asp:ListItem>
                            <asp:ListItem>Shipped</asp:ListItem>
                            <asp:ListItem>Complete</asp:ListItem>
                            <asp:ListItem>Cancelled</asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Status") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="PurchaseDate" HeaderText="PurchaseDate" SortExpression="PurchaseDate" DataFormatString="{0:d}" ReadOnly="true" />
                <asp:BoundField DataField="OrderTotal" HeaderText="OrderTotal" SortExpression="OrderTotal" ReadOnly="true" />
                <asp:BoundField DataField="TaxAmount" HeaderText="TaxAmount" SortExpression="TaxAmount" ReadOnly="true" />
                <asp:BoundField DataField="ShipAmount" HeaderText="ShipAmount" SortExpression="ShipAmount" ReadOnly="true" />
                <asp:BoundField DataField="ShipType" HeaderText="ShipType" SortExpression="ShipType" ReadOnly="true" />
                <asp:BoundField DataField="TrackingID" HeaderText="TrackingID" SortExpression="TrackingID" ReadOnly="true" />
                <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" ReadOnly="true" />
                <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName" ReadOnly="true" />
                <asp:BoundField DataField="ShipAddress1" HeaderText="ShipAddress1" SortExpression="ShipAddress1" ReadOnly="true" />
                <asp:BoundField DataField="ShipAddress2" HeaderText="ShipAddress2" SortExpression="ShipAddress2" ReadOnly="true" />
                <asp:BoundField DataField="ShipCity" HeaderText="ShipCity" SortExpression="ShipCity" ReadOnly="true" />
                <asp:BoundField DataField="ShipState" HeaderText="ShipState" SortExpression="ShipState" ReadOnly="true" />
                <asp:BoundField DataField="ShipZip" HeaderText="ShipZip" SortExpression="ShipZip" ReadOnly="true" />
                <asp:BoundField DataField="ShipCountry" HeaderText="ShipCountry" SortExpression="ShipCountry" ReadOnly="true" />
                <asp:BoundField DataField="CCID" HeaderText="CCID" SortExpression="CCID" ReadOnly="true" />
                <asp:BoundField DataField="PaymentType" HeaderText="PaymentType" SortExpression="PaymentType" ReadOnly="true" />
                <asp:CommandField ShowEditButton="True" ButtonType="Button" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="dsOrders" runat="server" ConnectionString="<%$ ConnectionStrings:myConnectionString %>"
            SelectCommand="SELECT * FROM [GadgetFox].[dbo].[Orders]"
            UpdateCommand="UPDATE [GadgetFox].[dbo].[Orders] SET Status=@Status WHERE OrderID=@OrderID">
            <UpdateParameters>
                <asp:Parameter Type="String" Name="OrderID" />
                <asp:Parameter Type="String" Name="ddlStatus" ConvertEmptyStringToNull="true" />
            </UpdateParameters>
        </asp:SqlDataSource>

</asp:Content>
