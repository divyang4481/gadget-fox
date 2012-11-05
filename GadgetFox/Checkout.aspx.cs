using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetFox
{
    public partial class Checkout : System.Web.UI.Page
    {
            Decimal cost;
            protected void Page_Load(object sender, EventArgs e)
            {
                if (Session["Cart"] == null)
                    Response.Redirect("ShoppingCart.aspx");
                BindBasket();
            }

            protected void RemoveProduct_Click(object sender, EventArgs e)
            {
                var cart = (List<string>)Session["Cart"];
                var removedProducts = Basket.Rows.Cast<GridViewRow>()
                  .Where(row => ((CheckBox)row.FindControl("RemovedProducts")).Checked)
                  .Select(row => Basket.DataKeys[row.RowIndex].Value.ToString()).ToList();
                cart.RemoveAll(removedProducts.Contains);
                BindBasket();
            }

            protected void BindBasket()
            {
                var sql = "SELECT ProductID, Name, Price FROM Products WHERE ProductID IN ({0})";
                var values = (List<string>)Session["Cart"];
                if (values.Count > 0)
                {
                    var parms = values.Select((s, i) => "@p" + i.ToString()).ToArray();
                    var inclause = string.Join(",", parms);
                    BasketData.SelectCommand = string.Format(sql, inclause);
                    BasketData.SelectParameters.Clear();
                    for (var i = 0; i < parms.Length; i++)
                    {
                        BasketData.SelectParameters.Add(parms[i].Replace("@", ""), values[i]);
                    }

                    DataView view = (DataView)BasketData.Select(DataSourceSelectArguments.Empty);

                    var costQuery = view.Cast<DataRowView>().Select(drv => drv.Row.Field<decimal>(" Price"));
                    cost = costQuery.Sum();
                    Basket.DataSource = view;
                    Basket.DataBind();
                }
            }

            protected void Basket_RowCreated(object sender, GridViewRowEventArgs e)
            {
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    Literal total = (Literal)e.Row.FindControl("TotalPrice");
                    total.Text = cost.ToString("c");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
