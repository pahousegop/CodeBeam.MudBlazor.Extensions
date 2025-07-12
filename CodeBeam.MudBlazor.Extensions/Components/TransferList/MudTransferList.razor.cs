﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class MudTransferList<T> : MudComponentBase
    {
        MudListExtended<T> _startList = new();
        MudListExtended<T> _endList = new();

        /// <summary>
        /// 
        /// </summary>
        protected string? StartClassname => new CssBuilder("mud-transfer-list-common")
            .AddClass(ClassListCommon)
            .AddClass(ClassStartList)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? EndClassname => new CssBuilder("mud-transfer-list-common")
            .AddClass(ClassListCommon)
            .AddClass(ClassEndList)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? StartStylename => new StyleBuilder()
            .AddStyle(StyleListCommon)
            .AddStyle(StyleStartList)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? EndStylename => new StyleBuilder()
            .AddStyle(StyleListCommon)
            .AddStyle(StyleEndList)
            .Build();

        /// <summary>
        /// The start list's collection.
        /// </summary>
        [Parameter]
        public ICollection<T?>? StartCollection { get; set; }

        /// <summary>
        /// Fires when start collection changed.
        /// </summary>
        [Parameter]
        public EventCallback<ICollection<T?>?> StartCollectionChanged { get; set; }

        /// <summary>
        /// The end list's collection.
        /// </summary>
        [Parameter]
        public ICollection<T?>? EndCollection { get; set; }

        /// <summary>
        /// Fires when end collection changed.
        /// </summary>
        [Parameter]
        public EventCallback<ICollection<T?>?> EndCollectionChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T?, string?>? ToStringFunc { get; set; }

        /// <summary>
        /// Fires before transfer process start. Useful to backup items or prevent transfer.
        /// </summary>
        [Parameter]
        public EventCallback OnTransferStart { get; set; }

        /// <summary>
        /// Fires when start collection changed. Takes a "StartToEnd" direction bool parameter.
        /// </summary>
        [Parameter]
        public Func<bool, bool>? PreventTransfer { get; set; }

        /// <summary>
        /// Fires when start collection changed. Takes a "StartToEnd" direction bool parameter.
        /// </summary>
        [Parameter]
        public Func<ICollection<T?>, ICollection<T?>>? OrderFunc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public RenderFragment? StartTitleContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public RenderFragment? EndTitleContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? StartTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? EndTitle { get; set; }

        /// <summary>
        /// If true, the transfer list will be displayed vertically. Useful for narrow spaces or mobile devices.
        /// </summary>
        [Parameter]
        public bool Vertical { get; set; }

        /// <summary>
        /// If true, adds top and bottom padding to the lists. Default is false.
        /// </summary>
        [Parameter]
        public bool Padding { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Parameter]
        public bool Dense { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool SearchBoxStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool SearchBoxEnd { get; set; }

        /// <summary>
        /// If true, double click transfers the item. Doesn't have any effect on multitransfer is true.
        /// </summary>
        [Parameter]
        public bool AllowDoubleClick { get; set; }

        /// <summary>
        /// Allows the transfer multiple items at once.
        /// </summary>
        [Parameter]
        public bool MultiSelection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public MultiSelectionComponent MultiSelectionComponent { get; set; } = MultiSelectionComponent.CheckBox;

        /// <summary>
        /// Select all types. If button is selected, 2 transfer all button appears. If Selectall item is selected, a list item appears.
        /// </summary>
        [Parameter]
        public SelectAllType SelectAllType { get; set; } = SelectAllType.Buttons;

        /// <summary>
        /// The color of lists and buttons. Default is primary.
        /// </summary>
        [Parameter]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The variant of buttons.
        /// </summary>
        [Parameter]
        public Variant ButtonVariant { get; set; } = Variant.Text;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? SelectAllText { get; set; } = "Select All";

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public int Spacing { get; set; } = 4;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public int ButtonSpacing { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public int? MaxItems { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? ClassStartList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? ClassEndList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? ClassListCommon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? StyleStartList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? StyleEndList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? StyleListCommon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startToEnd"></param>
        /// <returns></returns>
        protected internal async Task Transfer(bool startToEnd = true)
        {
            await OnTransferStart.InvokeAsync();
            if (PreventTransfer != null && PreventTransfer.Invoke(startToEnd) == true)
            {
                return;
            }
            if (startToEnd == true)
            {
                if (MultiSelection == false && _startList.SelectedValue != null)
                {
                    EndCollection?.Add(_startList.SelectedValue);
                    StartCollection?.Remove(_startList.SelectedValue);
                    OrderItems();
                    await EndCollectionChanged.InvokeAsync(EndCollection);
                    await StartCollectionChanged.InvokeAsync(StartCollection);
                    _endList.SelectedValue = _startList.SelectedValue;
                    _startList.Clear();
                    await _endList.ForceUpdate();
                }
                else if (MultiSelection == true && _startList.SelectedValues != null)
                {
                    ICollection<T> transferredValues = new List<T>();
                    foreach (var item in _startList.SelectedValues)
                    {
                        // This is not a great fix, but changing multiselection true after transfering a single selection item causes a null item transfer.
                        if (item == null)
                        {
                            continue;
                        }
                        EndCollection?.Add(item);
                        StartCollection?.Remove(item);
                        transferredValues.Add(item);
                    }
                    _endList.SelectedValues = transferredValues;
                    OrderItems();
                    await _endList.ForceUpdate();
                    _startList.Clear();
                    await EndCollectionChanged.InvokeAsync(EndCollection);
                    await StartCollectionChanged.InvokeAsync(StartCollection);
                }
                
            }
            else if (startToEnd == false)
            {
                if (MultiSelection == false && _endList.SelectedValue != null)
                {
                    StartCollection?.Add(_endList.SelectedValue);
                    EndCollection?.Remove(_endList.SelectedValue);
                    _startList.SelectedValue = _endList.SelectedValue;
                    _endList.Clear();
                    OrderItems();
                    await StartCollectionChanged.InvokeAsync(StartCollection);
                    await EndCollectionChanged.InvokeAsync(EndCollection);
                    if (OrderFunc != null)
                    {
                        await _startList.ForceUpdate();
                    }
                }
                else if (MultiSelection == true && _endList.SelectedValues != null)
                {
                    ICollection<T> transferredValues = new List<T>();
                    foreach (var item in _endList.SelectedValues)
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        StartCollection?.Add(item);
                        EndCollection?.Remove(item);
                        transferredValues.Add(item);
                    }
                    _startList.SelectedValues = transferredValues;
                    OrderItems();
                    await _startList.ForceUpdate();
                    _endList.Clear();
                    await StartCollectionChanged.InvokeAsync(StartCollection);
                    await EndCollectionChanged.InvokeAsync(EndCollection);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startToEnd"></param>
        /// <returns></returns>
        protected internal async Task TransferAll(bool startToEnd = true)
        {
            await OnTransferStart.InvokeAsync();
            if (PreventTransfer != null && PreventTransfer.Invoke(startToEnd) == true)
            {
                return;
            }
            if (startToEnd == true)
            {
                var transferredValues = new List<T>();
                foreach (var item in _startList.GetSearchedItems() ?? [])
                {
                    if (item != null)
                    {
                        transferredValues.Add(item);
                    }
                }
                
                foreach (var item in transferredValues ?? [])
                {
                    EndCollection?.Add(item);
                    StartCollection?.Remove(item);
                }
                OrderItems();
                await EndCollectionChanged.InvokeAsync(EndCollection);
                await StartCollectionChanged.InvokeAsync(StartCollection);
                _startList.Clear();
            }
            else if (startToEnd == false)
            {
                var transferredValues = new List<T>();
                foreach (var item in _endList.GetSearchedItems() ?? [])
                {
                    if (item != null)
                    {
                        transferredValues.Add(item);
                    }
                }

                foreach (var item in transferredValues ?? [])
                {
                    StartCollection?.Add(item);
                    EndCollection?.Remove(item);
                }
                OrderItems();
                await StartCollectionChanged.InvokeAsync(StartCollection);
                await EndCollectionChanged.InvokeAsync(EndCollection);
                _endList.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<T?>? GetStartListSelectedValues()
        {
            if (_startList == null)
            {
                return null;
            }

            if (MultiSelection == true)
            {
                return _startList.SelectedValues?.ToList();
            }
            else
            {
                return new List<T?>() { _startList.SelectedValue };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<T?>? GetEndListSelectedValues()
        {
            if (_endList == null)
            {
                return null;
            }

            if (MultiSelection == true)
            {
                return _endList.SelectedValues?.ToList();
            }
            else
            {
                return new List<T?>() { _endList.SelectedValue };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void OrderItems()
        {
            if (OrderFunc == null)
            {
                return;
            }
            StartCollection = OrderFunc.Invoke(StartCollection);
            EndCollection = OrderFunc.Invoke(EndCollection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task DoubleClick(ListItemClickEventArgs<T> args)
        {
            if (AllowDoubleClick == false)
            {
                return;
            }

            if (StartCollection != null && StartCollection.Contains(args.ItemValue))
            {
                await Transfer(true);
            }
            else if (EndCollection != null && EndCollection.Contains(args.ItemValue))
            {
                await Transfer(false);
            }

        }

    }
}
