using System;
using System.Collections.Generic;

namespace CafeReadModels
{
    public interface IOpenTabQueries
    {
        List<int> ActiveTableNumbers();
        TabInvoice InvoiceForTable(int table);
        Guid TabIdForTable(int table);
        TabStatus TabForTable(int table);
        Dictionary<int, List<TabItem>> TodoListForWaiter(string waiter);
    }
    public class TodoListItem
    {
        public int MenuNumber;
        public string Description;
    }

    public class TodoListGroup
    {
        public Guid Tab;
        public List<TodoListItem> Items;
    }
    public class TabItem
    {
        public int MenuNumber;
        public string Description;
        public decimal Price;
    }

    public class TabStatus
    {
        public Guid TabId;
        public int TableNumber;
        public List<TabItem> ToServe;
        public List<TabItem> InPreparation;
        public List<TabItem> Served;
    }

    public class TabInvoice
    {
        public Guid TabId;
        public int TableNumber;
        public List<TabItem> Items;
        public decimal Total;
        public bool HasUnservedItems;
    }
}
