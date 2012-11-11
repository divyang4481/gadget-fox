<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="GadgetFox.Cart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="inventoryGrid">
        <asp:GridView ID="gdvInventory" runat="server" SkinID="Professional" CellPadding="4" HeaderStyle-BackColor="#444444" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#dddddd" AutoGenerateColumns="False"
            AllowSorting="True" DataSourceID="SqlDataSource1" CssClass="inventoryGrid">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Image ID="Image1" runat="server" ImageUrl='<%# getImage(DataBinder.Eval(Container.DataItem, "ImageID").ToString()) %>' Height="150" Width="150" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Name" HeaderText="" SortExpression="Name" ReadOnly="True" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Label ID="lblPrice" runat="server" Text='<%# getPrice(DataBinder.Eval(Container.DataItem, "Price").ToString(),
                        DataBinder.Eval(Container.DataItem, "SalePrice").ToString(),Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "InSale").ToString())) %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Quantity" HeaderText="" SortExpression="Quantity" />
                <asp:CommandField ButtonType="Button" ShowEditButton="True" />
                <asp:CommandField ButtonType="Button" ShowDeleteButton="True" />
            </Columns>
            <EditRowStyle BackColor="#2461BF" />
            <EmptyDataTemplate>
                <asp:Label ID="lblNoData" runat="server" Text="Shopping cart is empty"></asp:Label>
            </EmptyDataTemplate>
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ ConnectionStrings:myConnectionString %>'
            SelectCommand="SELECT * FROM [viewCart] WHERE EmailID=@EmailID"
            UpdateCommand="UPDATE [GadgetFox].[dbo].[viewCart] SET Quantity=@Quantity WHERE ProductID=@ProductID and EmailID=@EmailID"
            DeleteCommand="DELETE FROM [GadgetFox].[dbo].[viewCart] WHERE ProductID=@ProductID and EmailID=@EmailID">
            <SelectParameters>
                <asp:Parameter Type="String" Name="EmailID"/>
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Type="String" Name="ProductID" />
                <asp:Parameter Type="String" Name="Quantity" />
                <asp:Parameter Type="String" Name="EmailID" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>

