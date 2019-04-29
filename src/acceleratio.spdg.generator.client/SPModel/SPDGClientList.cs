﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acceleratio.SPDG.Generator.SPModel;
using Microsoft.SharePoint.Client;

namespace Acceleratio.SPDG.Generator.Client.SPModel
{
    internal class SPDGClientList : SPDGList
    {
        private readonly SPDGWeb _web;
        private readonly List _list;
        private readonly ClientContext _context;

        public override string Title => _list.Title;

        public override string DefaultViewUrl => _list.DefaultViewUrl;

        public override bool IsDocumentLibrary => _list.BaseTemplate == 101;

        public override SPDGListTemplateType BaseTemplate => (SPDGListTemplateType)_list.BaseTemplate;

        public override SPDGFolder RootFolder
        {
            get
            {
                return new SPDGClientFolder(_list.RootFolder, _context);
            }
        }

        private void invalidateFields()
        {
            _fields = null;
        }
        private List<SPDGField> _fields;
        public override IEnumerable<SPDGField> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields=new List<SPDGField>();
                    _context.Load(_list.Fields, coll => coll.Include(x => x.Title, x => x.StaticName, x => x.InternalName));
                    _context.ExecuteQuery();
                    foreach (var field in _list.Fields)
                    {
                        _fields.Add(new SPDGField(field.Title, field.InternalName));
                    }
                }
                return _fields;
            }
        }

        public SPDGClientList(SPDGWeb web, List list, ClientContext context)
        {
            _web = web;
            _list = list;
            _context = context;
        }

        public static Expression<Func<List, object>>[] IncludeExpression
        {
            get
            {
                List<Expression<Func<List, object>>> includeExpression = new List<Expression<Func<List, object>>>();
                includeExpression.Add(web => web.Id);
                includeExpression.Add(web => web.Title);
                includeExpression.Add(web => web.DefaultViewUrl);
                includeExpression.Add(web => web.RootFolder);
                includeExpression.Add(web => web.HasUniqueRoleAssignments);
                includeExpression.Add(web => web.BaseTemplate);
                return includeExpression.ToArray();
            }
        }
        

        public override void AddFields(IEnumerable<SPDGFieldInfo> fields, bool addToDefaultView)
        {            
            invalidateFields();

            string schemaFormat = "<Field Type='{0}' DisplayName='{1}'/>";
            var addFieldOptions = AddFieldOptions.DefaultValue;
            if (addToDefaultView)
            {
                addFieldOptions |= AddFieldOptions.AddFieldToDefaultView;
            }
            foreach (var fieldInfo in fields)
            {
                _list.Fields.AddFieldAsXml(string.Format(schemaFormat, fieldInfo.FieldType, fieldInfo.DisplayName), false, addFieldOptions );
            }   
            _list.Update();
            _context.ExecuteQuery();
        }

        public override void AddItems(IEnumerable<ISPDGListItemInfo> items)
        {
            
            int counter = 0;
            foreach (var itemInfo in items)
            {
                counter++;
                var itemCreationInfo=new ListItemCreationInformation();
                var item=_list.AddItem(itemCreationInfo);
                foreach (var field in itemInfo.GetAvailableFields())
                {
                    item[field] = itemInfo[field];
                }
                item.Update();
                if (counter >=500)
                {
                    counter = 0;
                    _context.ExecuteQuery();
                }
            }
            _context.ExecuteQuery();
        }

        public override int DeleteItems(int numberOfItemsToDelete)
        {
            throw new NotImplementedException();
        }

        public override SPDGRoleAssignment GetRoleAssignmentByPrincipal(SPDGPrincipal principal)
        {
            return ClientRoleAssignmentHelper.GetRoleAssignmentByPrincipal(_list, _context, principal);
        }

        public override void AddRoleAssignment(SPDGPrincipal principal, IEnumerable<SPDGRoleDefinition> roledefinitions)
        {
            ClientRoleAssignmentHelper.AddRoleAssignment(_list, _context, principal, roledefinitions);
        }

        public override void RemoveRoleAssignment()
        {
            throw new NotImplementedException();
        }

        public override void RemoveRoleAssignment(SPDGPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public override int NumUniqueRoleAssignments
        {
            get { return _list.RoleAssignments.Count;  }
        }

        public override void BreakRoleInheritance(bool copyRoleDefinitions)
        {
            _list.BreakRoleInheritance(copyRoleDefinitions,false);   
            _context.ExecuteQuery();

        }

        public override bool HasUniqueRoleAssignments
        {
            get { return _list.HasUniqueRoleAssignments; }
        }

        private List<SPDGListItem> _items;
        public override IEnumerable<SPDGListItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = loadListItems();
                }
                return _items;
            }
        }



        private List<SPDGListItem> loadListItems()
        {
            List<SPDGListItem> retVal=new List<SPDGListItem>();
            ListItemCollectionPosition itemPosition = null;           
            do
            {
                CamlQuery query = CamlQuery.CreateAllItemsQuery(5000);
                query.ListItemCollectionPosition = itemPosition;
                var itemBatch = _list.GetItems(query);                 
                _context.Load(itemBatch, collection=>collection.Include(SPDGClientListItem.IncludeExpression), collection => collection.ListItemCollectionPosition);
                _context.ExecuteQuery();
                itemPosition = itemBatch.ListItemCollectionPosition;

                foreach (var item in itemBatch)
                {
                    retVal.Add(new SPDGClientListItem(item, _context));
                }

            } while (itemPosition != null);
           
           return retVal;
        }


        public override void AddView(string viewName, IEnumerable<string> viewFields, string strQuery, uint rowLimit, bool paged, bool makeDefault)
        {
            var viewCreationInfo=new ViewCreationInformation();
            viewCreationInfo.Title = viewName;
            viewCreationInfo.ViewFields = viewFields.ToArray();
            viewCreationInfo.Paged = paged;
            viewCreationInfo.RowLimit = rowLimit;
            viewCreationInfo.Query = strQuery;
            viewCreationInfo.SetAsDefaultView = makeDefault;
            _list.Views.Add(viewCreationInfo);
            _context.ExecuteQuery();            
        }

    }
}
