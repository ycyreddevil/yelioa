using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using System.Collections;
using Newtonsoft.Json.Linq;

/// <summary>
/// TreeNode 的摘要说明
/// </summary>
public class BudgetTreeNode
{
    public BudgetTreeNode(DataRow row)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        if (row == null)
            return;
        Id = Convert.ToInt32(row["Id"].ToString());
        FeeDetail = row["FeeDetail"].ToString();
        Budget = row["Budget"].ToString();
        state = "open";
        UsedAmount = row["UsedAmount"].ToString();
        children = new List<BudgetTreeNode>();
        listID = new ArrayList();
        if (!string.IsNullOrEmpty(row["ParentId"].ToString()))
            ParentId = Convert.ToInt32(row["ParentId"]);
        else
            ParentId = -1;
    }

    public BudgetTreeNode()
    {
        Id = 0;
        FeeDetail = null;
        state = "open";
        children = new List<BudgetTreeNode>();
    }
  
    public int Id { get; set; }  //节点的id值  
    public string FeeDetail { get; set; }  //节点显示的名称  
    public string Budget { get; set; }
    public string state { get; set; }//节点的状态  
    //public bool Checked { get; set; }
    public List<BudgetTreeNode> children { get; set; }  //集合属性，可以保存子节点  
    public string iconCls { get; set; }
    public string UsedAmount { get; set; }

    //附加属性
    public int MemberNumber { get; set; }
    public ArrayList listID { get; set; }
    public int ParentId { get; set; }
}
public class BudgetTreeNodeHelper
{
    List<BudgetTreeNode> budgetTree = null;

    public BudgetTreeNodeHelper(DataTable dt)
    {
        if(dt==null||dt.Rows.Count==0)
        {
            return ;
        }
      
        budgetTree = new List<BudgetTreeNode>();
   
        foreach (DataRow row in dt.Rows)
        {
            BudgetTreeNode node = new BudgetTreeNode(row);
            if(ContainsNode(budgetTree, node.Id))
            {
                continue;
            }
            if (string.IsNullOrEmpty(row["ParentId"].ToString()) || row["ParentId"].ToString()=="-1")
            {
                node.state = "open";
                budgetTree.Add(node);
            }
            else
            {
                AddNode(node, dt);
            }
        }
    }


    private void AddNode(BudgetTreeNode node,DataTable dt)
    {
        BudgetTreeNode parent = searchNode(node.ParentId);
        if (parent == null)
        {
            DataRow row = searchNode(dt,node.ParentId);
            if (row == null)
            {
                budgetTree.Add(node);
                return;
            }
                
            parent = new BudgetTreeNode(row);
            parent.children.Add(node);
            AddNode(parent,dt);
        }
        else
        {
            if (!ContainsNode(parent.children, node.Id))
                parent.children.Add(node);
        }
    }

    private bool ContainsNode(List<BudgetTreeNode> list, int id)
    {
        bool res = false;
        foreach (BudgetTreeNode node in list)
        {
            if (node.Id == id)
            {
                res = true;
                break;
            }
        }
        return res;
    }

    private DataRow searchNode(DataTable dt, int id)
    {
        foreach (DataRow row in dt.Rows)
        {
            if (Convert.ToInt32(row["Id"]) == id)
            {
                return row;
            }
        }
        return null;
    }

    private BudgetTreeNode searchNode(int id)
    {
        foreach(BudgetTreeNode node in budgetTree)
        {
            if(node.Id==id)
            {
                return node;
            }
        }
        return null;
    }
    public List<BudgetTreeNode> GetTree()
    {
        return budgetTree;
    }
    
}



