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
public class TreeNode
{
    public TreeNode(DataRow row)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        if (row == null)
            return;
        id = Convert.ToInt32(row["Id"].ToString());
        text = row["name"].ToString();
        state = "closed";
        //Checked = false;
        children = new List<TreeNode>();
        listID = new ArrayList();
        //MemberNumber = Convert.ToInt32(row["count"].ToString());
        if (!string.IsNullOrEmpty(row["parentId"].ToString()))
            parentId = Convert.ToInt32(row["parentId"]);
        else
            parentId = 0;
    }

    public TreeNode(string Text,int ID)
    {
        id = ID;
        text = Text;
        state = "open";
        //Checked = false;
        children = new List<TreeNode>();
        listID = new ArrayList();
        MemberNumber = 0;
    }
    public int id { get; set; }  //节点的id值  
    public string text { get; set; }  //节点显示的名称  
    public string state { get; set; }//节点的状态  
    //public bool Checked { get; set; }
    public List<TreeNode> children { get; set; }  //集合属性，可以保存子节点  
    public string iconCls { get; set; }

    //附加属性
    public int MemberNumber { get; set; }
    public ArrayList listID { get; set; }
    public int parentId { get; set; }
    //public string UserId { get; set; }


    //public void InitNode(DataRow row)
    //{

    //}
}

public class UserTreeHelper
{
    List<TreeNode> tree = null;
    DataTable dt = null;
    public UserTreeHelper(DataTable dt1)
    {        
        DataSet ds = UserInfoManage.GetUserDepartment();
        if(ds == null)
        {
            return;
        }
        DepartmentTreeHelper depart = new DepartmentTreeHelper(dt1);
        tree = depart.GetTree();
        dt = ds.Tables[0];
        InsertUserIntoTree(tree);
    }

    public UserTreeHelper(DataTable dt1,bool needInsertGroupInfo)
    {
        DataSet ds = UserInfoManage.GetUserDepartment();
        if (ds == null)
        {
            return;
        }
        DepartmentTreeHelper depart = new DepartmentTreeHelper(dt1);
        tree = depart.GetTree();
        dt = ds.Tables[0];
        InsertUserIntoTree(tree);
        if(needInsertGroupInfo)
            InsertGroupIntoTree();
    }



    private void InsertGroupIntoTree()
    {
        UserInfo user = (UserInfo)HttpContext.Current.Session["user"];
        DataTable dtGroup = EmailHelper.GetGroupDataTableInfo(user.userId.ToString());
        TreeNode groupRoot = new TreeNode("自定义分组信息", 1000000);
        foreach(DataRow row in dtGroup.Rows)
        {
            int id = 1000000 + Convert.ToInt32(row["Id"]) * 1000;
            TreeNode node = new TreeNode(row["GroupName"].ToString(), id);
            JArray users = JArray.Parse(row["GroupMember"].ToString());
            int index = 0;
            foreach(JObject jUser in users)
            {
                TreeNode nodeUser = new TreeNode(jUser["UserName"].ToString(), Convert.ToInt32(jUser["UserId"]));
                index++;
                nodeUser.iconCls = "icon-man";
                //nodeUser.UserId = jUser["UserId"].ToString();
                node.children.Add(nodeUser);
            }
            groupRoot.children.Add(node);
        }
        tree.Add(groupRoot);
    }

    private void InsertUserIntoTree(List<TreeNode> listNodes)
    {
        foreach(TreeNode node in listNodes)
        {
            if (node.children.Count > 0)
            {
                InsertUserIntoTree(node.children);
            }
            for(int i=dt.Rows.Count-1;i>=0;i--)
            {
                DataRow row = dt.Rows[i];
                if(node.id == Convert.ToInt32(row["departmentId"]))
                {
                    TreeNode n = new TreeNode(row["userName"].ToString(), Convert.ToInt32(row["userId"]));
                    n.iconCls = "icon-man";
                    node.children.Add(n);
                    dt.Rows.RemoveAt(i);
                }
            }
        }
    }

    private void InitTree(DataTable dt)
    {
        tree = new List<TreeNode>();
        int deep = 0;
        do
        {
            DataRow[] rows = dt.Select(string.Format("deep={0}", deep));
            if (rows.Length == 0)
            {
                break;
            }

            foreach (DataRow row in rows)
            {
                TreeNode node = new TreeNode(row);
                if (deep == 0)//插入根节点
                {
                    tree.Add(node);
                }
                else//插入普通节点
                {
                    TreeNode parent = FindNodeById(Convert.ToInt32(row["parentId"].ToString()));
                    if (parent == null)
                    {
                        continue;
                    }
                    parent.children.Add(node);
                }
            }
            deep++;

        } while (true);
        string range = "";
        foreach(TreeNode node in tree)
        {
            if(node.children.Count==0)
            {
                range += node.id + ",";
            }
        }
        DataSet ds = UserInfoManage.getUserTree(range);
        if(ds !=null)
        {
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                TreeNode node = new TreeNode(row["userName"].ToString(), Convert.ToInt32(row["userId"]));
                TreeNode parent = FindNodeById(Convert.ToInt32(row["departmentId"]));
                parent.children.Add(node);
            }
        }
    }

    private TreeNode FindNodeById(int id)
    {
        return SearchNodeList(tree, id);
    }

    private TreeNode SearchNodeList(List<TreeNode> listNodes, int id)
    {
        TreeNode res = null;
        foreach (TreeNode node in listNodes)
        {
            if (node.id == id)
            {
                return node;
            }
            else if (node.children.Count > 0)
            {
                TreeNode temp = SearchNodeList(node.children, id);
                if (temp != null)
                {
                    res = temp;
                    break;
                }
            }
        }
        return res;
    }


    public string GetJson()
    {
        if (tree == null)
            return "F";
        else
            return JsonConvert.SerializeObject(tree);
    }
}

public class DepartmentTreeHelper
{
    List<TreeNode> tree = null;
    DataTable Dt = null;
    public DepartmentTreeHelper(DataTable dt)
    {
        Dt = dt;
        if (Dt == null || Dt.Rows.Count == 0)
            return;
        InitTree(dt);
    }

    public DepartmentTreeHelper()
    {
        UserInfo user = (UserInfo)HttpContext.Current.Session["user"];
        DataSet ds = UserInfoManage.getTree(user.companyId.ToString());
        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
            return;
        }
        Dt = ds.Tables[0];
        InitTree(ds.Tables[0]);
    }
    //private void InitTree(DataTable dt)
    //{
    //    tree = new List<TreeNode>();
    //    int deep = 0;
    //    do
    //    {
    //        DataRow[] rows = dt.Select(string.Format("deep={0}", deep));
    //        if (rows.Length == 0)
    //        {
    //            break;
    //        }

    //        foreach (DataRow row in rows)
    //        {
    //            TreeNode node = new TreeNode(row);
    //            if (deep == 0)//插入根节点
    //            {
    //                tree.Add(node);
    //            }
    //            else//插入普通节点
    //            {
    //                TreeNode parent = FindNodeById(Convert.ToInt32(row["parentId"].ToString()));
    //                if (parent == null)
    //                {
    //                    continue;
    //                }
    //                parent.children.Add(node);
    //            }
    //        }
    //        deep++;

    //    } while (true);


    //    for (; deep >= 0; deep--)
    //    {
    //        DataRow[] rows = dt.Select(string.Format("deep={0}", deep));
    //        foreach (DataRow row in rows)
    //        {
    //            TreeNode node = FindNodeById(Convert.ToInt32(row["Id"].ToString()));
    //            node.listID.Add(node.id);
    //            foreach (TreeNode n in node.children)
    //            {
    //                node.listID.AddRange(n.listID);
    //            }
    //        }
    //    }

    //    Dictionary<string, string> dict = new Dictionary<string, string>();
    //    for (int i = 0; i < dt.Rows.Count; i++)
    //    {
    //        DataRow row = dt.Rows[i];
    //        TreeNode node = FindNodeById(Convert.ToInt32(row["Id"].ToString()));
    //        string departsId = "";
    //        foreach (object id in node.listID)
    //        {
    //            departsId += id.ToString() + ", ";
    //        }
    //        departsId = departsId.Substring(0, departsId.Length - 2);
    //        dict.Add(node.id.ToString(), departsId);
    //    }

    //    DataSet ds = UserInfoManage.getInfos(dict);
    //    for (int i = 0; i < dt.Rows.Count; i++)
    //    {
    //        DataRow row = dt.Rows[i];
    //        TreeNode node = FindNodeById(Convert.ToInt32(row["Id"].ToString()));
    //        node.MemberNumber = ds.Tables[i].Rows.Count;
    //    }
    //    return;
    //}

    private void InitTree(DataTable dt)
    {
        tree = new List<TreeNode>();
        DataRow rootRow = FindRootNode(dt);
        if (rootRow == null)
            return;
        TreeNode rootNode = new TreeNode(rootRow);
        rootNode.state = "open";
        tree.Add(rootNode);
        foreach (DataRow row in dt.Rows)
        {
            if (row["Id"].ToString() == rootRow["Id"].ToString())//跳过根节点，避免重复添加
                continue;
            TreeNode node = new TreeNode(row);
            if (node.parentId == 0)//根节点
            {
                tree.Add(node);
            }
            else
            {
                AddNode(node);
            }
        }

        //foreach (DataRow row in dt.Rows)
        //{
        //    int nodeId = Convert.ToInt32(row["Id"].ToString());
        //    TreeNode node = FindNodeById(nodeId);
        //    int[] Ids = GetAllSubNodeId(node).ToArray();
        //    foreach(int id in Ids)
        //    {
        //        DataRow subRow = FindRowById(dt, id);
        //        int num = Convert.ToInt32(subRow["count"]);
        //        node.MemberNumber += num;
        //    }
        //}
    }

    private List<int> GetAllSubNodeId(TreeNode node)
    {        
        List<int> list = new List<int>();
        foreach (TreeNode subNode in node.children)
        {
            list.Add(subNode.id);
            list.AddRange(GetAllSubNodeId(subNode));
        }
        return list;
    }

    private List<int> GetDepartIds(int id,List<int> departsId)
    {
        TreeNode node = FindNodeById(id);
        if (node != null)
        {
            if (!departsId.Contains(id))
                departsId.Add(id);
            if(node.parentId>0)//非根节点
                departsId = GetDepartIds(node.parentId, departsId);
        }            
        return departsId;
    }

    private bool ContainsNode(List<TreeNode> list,int id)
    {
        bool res = false;
        foreach(TreeNode node in list)
        {
            if(node.id == id)
            {
                res = true;
                break;
            }
        }
        return res;
    }

    private void AddNode(TreeNode node)
    {
        //if(node.id==113)
        //{
        //    node.state = "open";
        //}
        TreeNode parent = FindNodeById(node.parentId);
        if (parent == null)
        {
            DataRow row = FindRowById(node.parentId);
            if (row == null)
                return;
            parent = new TreeNode(row);
            parent.children.Add(node);            
            AddNode(parent);
        }
        else
        {
            if(!ContainsNode(parent.children,node.id))
                parent.children.Add(node);
            //if(!parent.listID.Contains(node.id))         
            //    parent.listID.Add(node.id);
            //foreach (TreeNode n in node.children)
            //{
            //    if (!parent.listID.Contains(node.id))
            //        node.listID.AddRange(n.listID);
            //}
        }
    }

    private DataRow FindRootNode(DataTable dt)
    {
        foreach(DataRow row in dt.Rows)
        {
            if (Convert.ToInt32(row["parentId"])==0)
                return row;
        }
        return null;
    }

    private DataRow FindRowById(DataTable dt,int id)
    {
        foreach(DataRow row in dt.Rows)
        {
            if(Convert.ToInt32(row["Id"]) == id)
            {
                return row;
            }
        }
        return null;
    }

    private DataRow FindRowById(int id)
    {
        return FindRowById(Dt, id);
    }

    private TreeNode FindNodeById(int id)
    {        
        return SearchNodeList(tree,id);
    }

    private TreeNode SearchNodeList(List<TreeNode> listNodes, int id)
    {
        TreeNode res = null;
        foreach (TreeNode node in listNodes)
        {
            if (node.id == id)
            {
                return node;
            }
            else if(node.children.Count > 0)
            {
                TreeNode temp = SearchNodeList(node.children, id);
                if (temp != null)
                {
                    res = temp;
                    break;
                }
            }
        }
        return res;
    }
    
    public List<TreeNode> GetTree()
    {
        return tree;
    }
    public string GetJson()
    {
        if (tree == null)
            return "F";
        else
            return JsonConvert.SerializeObject(tree);
    }
}