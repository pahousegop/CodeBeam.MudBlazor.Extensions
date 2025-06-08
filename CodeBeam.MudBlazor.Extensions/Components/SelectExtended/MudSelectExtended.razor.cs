﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Exceptions;

namespace MudExtensions
{
    /// <summary>
    /// Select component with advanced features.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class MudSelectExtended<T> : MudBaseInputExtended<T>, IMudSelectExtended, IMudShadowSelectExtended
    {

        #region Constructor, Injected Services, Parameters, Fields

        /// <summary>
        /// 
        /// </summary>
        public MudSelectExtended()
        {
            Adornment = Adornment.End;
            IconSize = Size.Medium;
        }

        [Inject] private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        private MudListExtended<T?>? _list;
        private bool _dense;
        private string? multiSelectionText;
        /// <summary>
        /// The collection of items within this select
        /// </summary>
        public IReadOnlyList<MudSelectItemExtended<T?>>? Items => _items;

        /// <summary>
        /// 
        /// </summary>
        protected internal List<MudSelectItemExtended<T?>> _items = new();
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<T, MudSelectItemExtended<T?>> _valueLookup = new();
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<T, MudSelectItemExtended<T?>> _shadowLookup = new();
        private MudInputExtended<string> _elementReference = new();
        internal bool _isOpen;
        /// <summary>
        /// 
        /// </summary>
        protected internal string? _currentIcon { get; set; }
        internal event Action<ICollection<T?>>? SelectionChangedFromOutside;

        /// <summary>
        /// 
        /// </summary>
        protected string? Classname =>
            new CssBuilder("mud-select-extended")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? InputClassname =>
            new CssBuilder("mud-select-input-extended")
            .AddClass("mud-select-extended-nowrap", NoWrap)
            .AddClass("mud-no-start-adornment", AdornmentStart == null)
            .AddClass(InputClass)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? InputChipClassname =>
            new CssBuilder("mud-select-input-chip-extended")
            .AddClass("mud-select-extended-nowrap mud-chip-scroll-container", NoWrap)
            .Build();

        private string _elementId = "select_" + Guid.NewGuid().ToString().Substring(0, 8);
        private string _popoverId = "selectpopover_" + Guid.NewGuid().ToString().Substring(0, 8);

        /// <summary>
        /// User class names for the input, separated by space
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter] public string? InputClass { get; set; }

        /// <summary>
        /// User style names for the input, separated by space
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter] public string? InputStyle { get; set; }

        /// <summary>
        /// Fired when dropdown opens.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Behavior)]
        [Parameter] public EventCallback OnOpen { get; set; }

        /// <summary>
        /// Fired when dropdown closes.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Behavior)]
        [Parameter] public EventCallback OnClose { get; set; }

        /// <summary>
        /// Add the MudSelectItems here
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Optional additional content to display above the list within the popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public RenderFragment<MudSelectExtended<T>>? StaticContent { get; set; }

        /// <summary>
        /// Whether to show <see cref="StaticContent"/> at the bottom of the popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShowStaticContentAtEnd { get; set; }

        /// <summary>
        /// Optional presentation template for items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public RenderFragment<MudListItemExtended<T?>>? ItemTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for selected items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public RenderFragment<MudListItemExtended<T?>>? ItemSelectedTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for disabled items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public RenderFragment<MudListItemExtended<T?>>? ItemDisabledTemplate { get; set; }

        /// <summary>
        /// Function to be invoked when checking whether an item should be disabled or not. Works both with renderfragment and ItemCollection approach.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T?, bool>? ItemDisabledFunc { get; set; }

        /// <summary>
        /// Classname for item template or chips.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public string? TemplateClass { get; set; }

        /// <summary>
        /// If true the active (hilighted) item select on tab key. Designed for only single selection. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool SelectValueOnTab { get; set; } = true;

        /// <summary>
        /// If false multiline text show. Default is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool NoWrap { get; set; }

        /// <summary>
        /// User class names for the popover, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public bool EnablePopoverPadding { get; set; } = true;

        /// <summary>
        /// If true, selected items doesn't have a selected background color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool EnableSelectedItemStyle { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? SearchBoxPlaceholder { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all Select items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public bool Dense
        {
            get { return _dense; }
            set { _dense = value; }
        }

        /// <summary>
        /// The Open Select Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// Dropdown color of select. Supports theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The Close Select Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// The value presenter.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public ValuePresenter ValuePresenter { get; set; } = ValuePresenter.Text;

        /// <summary>
        /// If set to true and the MultiSelection option is set to true, a "select all" checkbox is added at the top of the list of items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool SelectAll { get; set; }

        /// <summary>
        /// Sets position of the Select All checkbox
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public SelectAllPosition SelectAllPosition { get; set; } = SelectAllPosition.BeforeSearchBox;

        /// <summary>
        /// Define the text of the Select All option.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? SelectAllText { get; set; } = "Select All";

        /// <summary>
        /// Function to define a customized multiselection text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Func<List<T?>, string?>? MultiSelectionTextFunc { get; set; }

        /// <summary>
        /// Custom search func for searchbox. If doesn't set, it search with "Contains" logic by default. Passed value and searchString values.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T?, string?, bool>? SearchFunc { get; set; }

        /// <summary>
        /// If not null, select items will automatically created regard to the collection.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public ICollection<T?>? ItemCollection { get; set; } = null;

        /// <summary>
        /// Allows virtualization. Only work is ItemCollection parameter is not null.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Virtualize { get; set; }

        /// <summary>
        /// If true, chips has close button and remove from SelectedValues when pressed the close button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool ChipCloseable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? ChipClass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Variant ChipVariant { get; set; } = Variant.Filled;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Size ChipSize { get; set; } = Size.Small;

        /// <summary>
        /// Parameter to define the delimited string separator.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Delimiter { get; set; } = ", ";

        /// <summary>
        /// If true popover width will be the same as the select component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public DropdownWidth RelativeWidth { get; set; } = DropdownWidth.Relative;

        /// <summary>
        /// Sets the maxheight the Select can have when open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// Set the anchor origin point to determen where the popover will open from.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin AnchorOrigin { get; set; } = Origin.TopCenter;

        /// <summary>
        /// Sets the transform origin point for the popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopCenter;

        /// <summary>
        /// If true, the Select's input will not show any values that are not defined in the dropdown.
        /// This can be useful if Value is bound to a variable which is initialized to a value which is not in the list
        /// and you want the Select to show the label / placeholder instead.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Strict { get; set; }

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// If true, shows a searchbox for filtering items. Only works with ItemCollection approach.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool SearchBox { get; set; }

        /// <summary>
        /// If true, the search-box will be focused when the dropdown is opened.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool SearchBoxAutoFocus { get; set; }

        /// <summary>
        /// If true, the search-box has a clear icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool SearchBoxClearable { get; set; }

        /// <summary>
        /// Search box text field variant.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Variant SearchBoxVariant { get; set; } = Variant.Outlined;

        /// <summary>
        /// Search box icon position.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Adornment SearchBoxAdornment { get; set; } = Adornment.End;

        /// <summary>
        /// Fired when the search value changes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<string> OnSearchStringChange { get; set; }

        /// <summary>
        /// If true, prevent scrolling while dropdown is open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool LockScroll { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// Custom checked icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// Custom unchecked icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// Custom indeterminate icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        private bool _multiSelection = false;
        /// <summary>
        /// If true, multiple values can be selected via checkboxes which are automatically shown in the dropdown
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool MultiSelection
        {
            get => _multiSelection;
            set
            {
                if (value != _multiSelection)
                {
                    _multiSelection = value;
                    UpdateTextPropertyAsync(false).CatchAndLog();
                }
            }
        }

        /// <summary>
        /// The MultiSelectionComponent's placement. Accepts Align.Start and Align.End
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Align MultiSelectionAlign { get; set; } = Align.Start;

        /// <summary>
        /// The component which shows as a MultiSelection check.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public MultiSelectionComponent MultiSelectionComponent { get; set; } = MultiSelectionComponent.CheckBox;

        private IEqualityComparer<T?>? _comparer;
        /// <summary>
        /// The Comparer to use for comparing selected values internally.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public IEqualityComparer<T?>? Comparer
        {
            get => _comparer;
            set
            {
                if (_comparer == value)
                    return;
                _comparer = value;
                // Apply comparer and refresh selected values
                _selectedValues = new HashSet<T?>(_selectedValues, _comparer);
                SelectedValues = _selectedValues;
            }
        }

        private Func<T?, string?>? _toStringFunc = x => x?.ToString();
        /// <summary>
        /// Defines how values are displayed in the drop-down list
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T?, string?>? ToStringFunc
        {
            get => _toStringFunc;
            set
            {
                if (_toStringFunc == value)
                    return;
                _toStringFunc = value;
                Converter = new Converter<T?>
                {
                    SetFunc = _toStringFunc ?? (x => x?.ToString()),
                };
            }
        }

        #endregion


        #region Values, Texts & Items

        //This 'started' field is only for fixing multiselect keyboard navigation test. Has a very minor effect, so can be removable for a better gain.
        private bool _selectedValuesSetterStarted = false;
        private HashSet<T?>? _selectedValues;
        /// <summary>
        /// Set of selected values. If MultiSelection is false it will only ever contain a single value. This property is two-way bindable.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public IEnumerable<T?>? SelectedValues
        {
            get
            {
                if (_selectedValues == null)
                    _selectedValues = new HashSet<T?>(_comparer);
                return _selectedValues;
            }
            set
            {
                var set = value ?? new HashSet<T?>(_comparer);
                if (value == null && _selectedValues == null)
                {
                    return;
                }
                if (value != null && _selectedValues != null && _selectedValues.SetEquals(value))
                {
                    return;
                }
                if (SelectedValues?.Count() == set.Count() && _selectedValues?.All(x => set.Contains(x, _comparer)) == true)
                    return;

                if (_selectedValuesSetterStarted)
                {
                    return;
                }
                _selectedValuesSetterStarted = true;
                _selectedValues = new HashSet<T?>(set, _comparer);
                SelectionChangedFromOutside?.Invoke(new HashSet<T?>(_selectedValues, _comparer));
                if (!MultiSelection)
                {
                    SetValueAsync(_selectedValues.FirstOrDefault()).CatchAndLog();
                }
                else
                {
                    SetValueAsync(_selectedValues.LastOrDefault(), false).CatchAndLog();
                    UpdateTextPropertyAsync(false).CatchAndLog();
                }

                SelectedValuesChanged.InvokeAsync(new HashSet<T?>(SelectedValues, _comparer)).CatchAndLog();
                _selectedValuesSetterStarted = false;
                //Console.WriteLine("SelectedValues setter ended");
            }
        }

        private MudListItemExtended<T?>? _selectedListItem;
        private HashSet<MudListItemExtended<T?>>? _selectedListItems;

        /// <summary>
        /// 
        /// </summary>
        protected internal MudListItemExtended<T?>? SelectedListItem
        {
            get => _selectedListItem;

            set
            {
                if (_selectedListItem == value)
                {
                    return;
                }
                _selectedListItem = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal IEnumerable<MudListItemExtended<T?>>? SelectedListItems
        {
            get => _selectedListItems;

            set
            {
                if (value == null && _selectedListItems == null)
                {
                    return;
                }

                if (value != null && _selectedListItems != null && _selectedListItems.SetEquals(value))
                {
                    return;
                }
                _selectedListItems = value == null ? null : value.ToHashSet();
            }
        }

        /// <summary>
        /// Fires when SelectedValues changes.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<T?>?> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Should only be used for debugging and development purposes.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<MudListItemExtended<T?>>> SelectedListItemsChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="updateValue"></param>
        /// <param name="selectedConvertedValues"></param>
        /// <param name="multiSelectionTextFunc"></param>
        /// <returns></returns>
        protected async Task SetCustomizedTextAsync(string? text, bool updateValue = true,
            List<T?>? selectedConvertedValues = null,
            Func<List<T?>, string?>? multiSelectionTextFunc = null)
        {
            // The Text property of the control is updated
            Text = multiSelectionTextFunc?.Invoke(selectedConvertedValues);

            // The comparison is made on the multiSelectionText variable
            if (multiSelectionText != text)
            {
                multiSelectionText = text;
                if (!string.IsNullOrWhiteSpace(multiSelectionText))
                    Touched = true;
                if (updateValue)
                    await UpdateValuePropertyAsync(false);
                await TextChanged.InvokeAsync(multiSelectionText);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateText"></param>
        /// <returns></returns>
        protected override Task UpdateValuePropertyAsync(bool updateText)
        {
            // For MultiSelection of non-string T's we don't update the Value!!!
            //if (typeof(T) == typeof(string) || !MultiSelection)
            base.UpdateValuePropertyAsync(updateText).CatchAndLog();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        protected override Task UpdateTextPropertyAsync(bool updateValue)
        {
            List<string?> textList = new List<string?>();
            if (Items != null && Items.Any())
            {
                if (ItemCollection != null)
                {
                    foreach (var val in SelectedValues ?? new List<T?>())
                    {
                        var collectionValue = ItemCollection.FirstOrDefault(x => x != null && (Comparer != null ? Comparer.Equals(x, val) : x.Equals(val)));
                        if (collectionValue != null)
                        {
                            textList.Add(Converter.Set(collectionValue));
                        }
                    }
                }
                else
                {
                    foreach (var val in SelectedValues ?? new List<T?>())
                    {
                        if (!Strict && !Items.Select(x => x.Value).Contains(val))
                        {
                            textList.Add(ToStringFunc != null ? ToStringFunc(val) : Converter.Set(val));
                            continue;
                        }
                        var item = Items.FirstOrDefault(x => x != null && (x.Value == null ? val == null : Comparer != null ? Comparer.Equals(x.Value, val) : x.Value.Equals(val)));
                        if (item != null)
                        {
                            textList.Add(!string.IsNullOrEmpty(item.Text) ? item.Text : Converter.Set(item.Value));
                        }
                    }
                }
            }

            // when multiselection is true, we return
            // a comma separated list of selected values
            if (MultiSelection)
            {
                if (MultiSelectionTextFunc != null)
                {
                    return SetCustomizedTextAsync(string.Join(Delimiter, textList),
                        selectedConvertedValues: SelectedValues?.ToList(),
                        multiSelectionTextFunc: MultiSelectionTextFunc, updateValue: updateValue);
                }
                else
                {
                    return SetTextAsync(string.Join(Delimiter, textList), updateValue: updateValue);
                }
            }
            else
            {
                var item = Items?.FirstOrDefault(x => Value == null ? x.Value == null : Comparer != null ? Comparer.Equals(Value, x.Value) : Value.Equals(x.Value));
                if (item == null)
                {
                    return SetTextAsync(Converter.Set(Value), false);
                }
                return SetTextAsync((!string.IsNullOrEmpty(item.Text) ? item.Text : Converter.Set(item.Value)), updateValue: updateValue);
            }
        }

        private string? GetSelectTextPresenter()
        {
            return Text;
        }

        #endregion


        #region Lifecycle Methods

        /// <summary>
        /// 
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateIcon();
            if (!MultiSelection && Value != null)
            {
                _selectedValues = new HashSet<T?>(_comparer) { Value };
            }
            else if (MultiSelection && SelectedValues != null)
            {
                // TODO: Check this line again
                SetValueAsync(SelectedValues.FirstOrDefault()).CatchAndLog();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            UpdateIcon();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                var options = new KeyInterceptorOptions(
                    "mud-input-control",
                    [
                        // prevent scrolling page, toggle open/close
                        new(" ", preventDown: "key+none"),
                        // prevent scrolling page, instead highlight previous item
                        new("ArrowUp", preventDown: "key+none"),
                        // prevent scrolling page, instead highlight next item
                        new("ArrowDown", preventDown: "key+none"),
                        new("Home", preventDown: "key+none"),
                        new("End", preventDown: "key+none"),
                        new("Escape"),
                        new("Enter", preventDown: "key+none"),
                        new("NumpadEnter", preventDown: "key+none"),
                        // select all items instead of all page text
                        new("a", preventDown: "key+ctrl"),
                        // select all items instead of all page text
                        new("A", preventDown: "key+ctrl"),
                        // for our users
                        new("/./", subscribeDown: true, subscribeUp: true)
                    ]);

                await KeyInterceptorService.SubscribeAsync(_elementId, options, keyDown: HandleKeyDownAsync, keyUp: HandleKeyUpAsync);

                await UpdateTextPropertyAsync(false);
                _list?.ForceUpdateItems();
                SelectedListItem = Items.FirstOrDefault(x => x.Value != null && Value != null && x.Value.Equals(Value))?.ListItem;
                StateHasChanged();
            }
            //Console.WriteLine("Select rendered");
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Refresh all items.
        /// </summary>
        public void ForceUpdateItems()
        {
            _list?.ForceUpdateItems();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            if (IsJSRuntimeAvailable)
            {
                await KeyInterceptorService.UnsubscribeAsync(_elementId);
            }
        }

        #endregion


        #region Events (Key, Focus)

        /// <summary>
        /// Keydown event.
        /// </summary>
        /// <param name="obj"></param>
        protected internal async Task HandleKeyDownAsync(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;

            if (_list != null && _isOpen)
            {
                await _list.HandleKeyDownAsync(obj);
            }

            switch (obj.Key)
            {
                case "Tab":
                    await CloseMenu();
                    break;
                case "ArrowUp":
                    if (obj.AltKey)
                    {
                        await CloseMenu();
                    }
                    else if (!_isOpen)
                    {
                        await OpenMenu();
                    }
                    break;
                case "ArrowDown":
                    if (obj.AltKey)
                    {
                        await OpenMenu();
                    }
                    else if (!_isOpen)
                    {
                        await OpenMenu();
                    }
                    break;
                case " ":
                    await ToggleMenu();
                    break;
                case "Escape":
                    await CloseMenu();
                    break;
                case "Enter":
                case "NumpadEnter":
                    if (!MultiSelection)
                    {
                        if (!_isOpen)
                        {
                            await OpenMenu();
                        }
                        else
                        {
                            await CloseMenu();
                        }
                        break;
                    }
                    else
                    {
                        if (!_isOpen)
                        {
                            await OpenMenu();
                            break;
                        }
                        else
                        {
                            await _elementReference.SetText(Text);
                            break;
                        }
                    }
            }
            await OnKeyDown.InvokeAsync(obj);

        }

        /// <summary>
        /// Keyup event.
        /// </summary>
        /// <param name="obj"></param>
        protected internal async Task HandleKeyUpAsync(KeyboardEventArgs obj)
        {
            await OnKeyUp.InvokeAsync(obj);
        }

        /// <summary>
        /// Blur event.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal async Task OnLostFocus(FocusEventArgs obj)
        {
            //if (_isOpen)
            //{
            //    // when the menu is open we immediately get back the focus if we lose it (i.e. because of checkboxes in multi-select)
            //    // otherwise we can't receive key strokes any longer
            //    _elementReference.FocusAsync().AndForget(TaskOption.Safe);
            //}
            //base.OnBlur.InvokeAsync(obj).AndForget();

            await OnBlurredAsync(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ValueTask BlurAsync()
        {
            return _elementReference.BlurAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        #endregion


        #region PopoverState

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ToggleMenu()
        {
            if (Disabled || ReadOnly)
                return;
            if (_isOpen)
                await CloseMenu();
            else
                await OpenMenu();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task OpenMenu()
        {
            if (Disabled || ReadOnly)
                return;
            _isOpen = true;
            UpdateIcon();
            StateHasChanged();

            //disable escape propagation: if selectmenu is open, only the select popover should close and underlying components should not handle escape key
            await KeyInterceptorService.UpdateKeyAsync(_elementId, new() { Key = "Escape", StopDown = "Key+none" });
            await OnOpen.InvokeAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task CloseMenu()
        {
            _isOpen = false;
            UpdateIcon();
            StateHasChanged();
            //if (focusAgain == true)
            //{
            //    StateHasChanged();
            //    await OnBlur.InvokeAsync(new FocusEventArgs());
            //    _elementReference.FocusAsync().AndForget(TaskOption.Safe);
            //    StateHasChanged();
            //}

            //enable escape propagation: the select popover was closed, now underlying components are allowed to handle escape key
            await KeyInterceptorService.UpdateKeyAsync(_elementId, new() { Key = "Escape", StopDown = "none" });

            await OnClose.InvokeAsync();
        }

        #endregion


        #region Item Registration & Selection

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task SelectOption(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                if (!MultiSelection)
                    await CloseMenu();
                return;
            }
            await SelectOption(_items[index].Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task SelectOption(object? obj)
        {
            var value = (T?)obj;
            if (MultiSelection)
            {
                await UpdateTextPropertyAsync(false);
                //UpdateSelectAllChecked();
                await BeginValidateAsync();
            }
            else
            {
                // CloseMenu(true) doesn't close popover in BSS
                await CloseMenu();

                if (EqualityComparer<T>.Default.Equals(Value, value))
                {
                    StateHasChanged();
                    return;
                }

                await SetValueAsync(value);
                //await UpdateTextPropertyAsync(false);
                _elementReference.SetText(Text).CatchAndLog();
                //_selectedValues.Clear();
                //_selectedValues.Add(value);
            }

            //await SelectedValuesChanged.InvokeAsync(SelectedValues);
            await InvokeAsync(StateHasChanged);
        }

        //TODO: will override this method when core library will have the base one.
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task ForceUpdate()
        {
            await base.ForceUpdate();
            if (!MultiSelection)
            {
                SelectedValues = new HashSet<T?>(_comparer) { Value };
            }
            else
            {
                await SelectedValuesChanged.InvokeAsync(new HashSet<T?>(SelectedValues, _comparer));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task BeginValidatePublic()
        {
            await BeginValidateAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected internal bool Add(MudSelectItemExtended<T?>? item)
        {
            if (item == null)
                return false;
            bool? result = null;
            if (!_items.Select(x => x.Value).Contains(item.Value))
            {
                _items.Add(item);

                if (item.Value != null)
                {
                    _valueLookup[item.Value] = item;
                    if (item.Value.Equals(Value) && !MultiSelection)
                        result = true;
                }
            }
            //UpdateSelectAllChecked();
            if (!result.HasValue)
            {
                result = item.Value?.Equals(Value);
            }
            return result == true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected internal void Remove(MudSelectItemExtended<T?>? item)
        {
            if (item == null)
            {
                return;
            }
            _items.Remove(item);
            if (item.Value != null)
                _valueLookup.Remove(item.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void RegisterShadowItem(MudSelectItemExtended<T?>? item)
        {
            if (item == null || item.Value == null)
                return;
            _shadowLookup[item.Value] = item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void UnregisterShadowItem(MudSelectItemExtended<T?>? item)
        {
            if (item == null || item.Value == null)
                return;
            _shadowLookup.Remove(item.Value);
        }

        #endregion


        #region Clear

        /// <summary>
        /// Extra handler for clearing selection.
        /// </summary>
        protected async ValueTask SelectClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetValueAsync(default, false);
            await SetTextAsync(default, false);
            _selectedValues?.Clear();
            SelectedListItem = null;
            SelectedListItems = null;
            await BeginValidateAsync();
            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(new HashSet<T?>(SelectedValues, _comparer));
            await OnClearButtonClick.InvokeAsync(e);
        }

        /// <summary>
        /// Clear the selection
        /// </summary>
        public async Task Clear()
        {
            await SetValueAsync(default, false);
            await SetTextAsync(default, false);
            _selectedValues?.Clear();
            await BeginValidateAsync();
            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(new HashSet<T?>(SelectedValues, _comparer));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task ResetValueAsync()
        {
            await base.ResetValueAsync();
            SelectedValues = null;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected bool IsValueInList
        {
            get
            {
                if (Value == null)
                    return false;
                //return _shadowLookup.TryGetValue(Value, out var _);
                foreach (var value in Items?.Select(x => x.Value) ?? new List<T?>())
                {
                    if (Comparer != null ? Comparer.Equals(value, Value) : value?.Equals(Value) == true) //(Converter.Set(item.Value) == Converter.Set(Value))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void UpdateIcon()
        {
            _currentIcon = !string.IsNullOrWhiteSpace(AdornmentIcon) ? AdornmentIcon : _isOpen ? CloseIcon : OpenIcon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select_item"></param>
        /// <exception cref="GenericTypeMismatchException"></exception>
        public void CheckGenericTypeMatch(object select_item)
        {
            var itemT = select_item.GetType().GenericTypeArguments[0];
            if (itemT != typeof(T))
                throw new GenericTypeMismatchException("MudSelectExtended", "MudSelectItemExtended", typeof(T), itemT);
        }

        /// <summary>
        /// Fixes issue #4328
        /// Returns true when MultiSelection is true and it has selected values(Since Value property is not used when MultiSelection=true
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True when component has a value</returns>
        protected override bool HasValue(T? value)
        {
            if (MultiSelection)
                return SelectedValues?.Count() > 0;
            else
                return base.HasValue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chip"></param>
        /// <returns></returns>
        protected async Task ChipClosed(MudChip<T>? chip)
        {
            if (chip == null || SelectedValues == null)
            {
                return;
            }
            //SelectedValues = SelectedValues.Where(x => Converter.Set(x)?.ToString() != chip.Value?.ToString());
            SelectedValues = SelectedValues.Where(x => x?.Equals(chip.Value) == false);
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
        }
        
        /// <summary>
        /// returns the value of the internal property _isOpen 
        /// </summary>
        /// <returns></returns>
        public bool GetOpenState()
        {
            return _isOpen;
        }
    }
}
