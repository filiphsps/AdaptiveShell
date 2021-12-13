using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

// Thanks to https://www.codeproject.com/Articles/536519/Extending-GridView-with-Drag-and-Drop-for-Grouping
// and https://www.codeproject.com/Articles/1037059/How-to-Upgrade-Extended-GridView-from-WinRT-to-Uni
namespace Shell.Controls {
    /// <summary>
    /// The <see cref="GridViewExControl"/> class implements drag&drop for cases which are not supported by the <see cref="GridView"/> control:
    /// - for ItemsPanels other than StackPanel, WrapGrid, VirtualizingStackPanel;
    /// - for cases when grouping is set.
    /// It also allows adding new groups to the underlying datasource if end-user drags some item to the left-most or the rigt-most sides of the control.
    /// </summary>
    /// <remarks>
    /// To allow new group creation by the end-user, set <see cref="GridViewExControl.AllowNewGroup"/> property to true.
    /// To add new group, handle <see cref="GridViewExControl.BeforeDrop"/> event. The <see cref="BeforeDropItemsEventArgs.RequestCreateNewGroup"/>
    /// property value defines whether the new group creation has been requested by the end-user actions.
    /// If this property is true, create the new data group and insert it into the groups collection at the positions, specified by the
    /// <see cref="BeforeDropItemsEventArgs.NewGroupIndex"/> property value. Then the <see cref="GridViewExControl"/> will insert dragged item
    /// into the newly added group.
    /// Note: you should create new group from your code, as the <see cref="GridViewExControl"/> control knows nothing about your data structure.
    /// </remarks>
    [TemplatePart(Name = GridViewExControl.NewGroupPlaceholderFirstName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = GridViewExControl.NewGroupPlaceholderLastName, Type = typeof(FrameworkElement))]
    public class GridViewExControl : GridView {
        //-------------------------------------------------------------
        #region ** Template Parts
        private const String NewGroupPlaceholderFirstName = "NewGroupPlaceholderFirst";
        private FrameworkElement _NewGroupPlaceholderFirst;

        private const String NewGroupPlaceholderLastName = "NewGroupPlaceholderLast";
        private FrameworkElement _NewGroupPlaceholderLast;

        #endregion

        //----------------------------------------------------------------------
        #region ** dependency properties
        /// <summary>
        /// Gets or sets the <see cref="Boolean"/> value determining whether new group should be created at dragging the item to the empty space.
        /// This is a dependency property. The default value is false.
        /// </summary>
        /// 
        public UIElement Root { get; set; }

        public Boolean AllowNewGroup {
            get { return (Boolean)this.GetValue(AllowNewGroupProperty); }
            set { this.SetValue(AllowNewGroupProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AllowNewGroup"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowNewGroupProperty =
            DependencyProperty.Register("AllowNewGroup", typeof(Boolean), typeof(GridViewExControl), new PropertyMetadata(false));
        #endregion

        //----------------------------------------------------------------------
        #region ** events
        /// <summary>
        /// Occurs before performing drop operation,
        /// </summary>
        public event EventHandler<BeforeDropItemsEventArgs> BeforeDrop;
        /// <summary>
        /// Rizes the <see cref="BeforeDrop"/> event.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected virtual void OnBeforeDrop(BeforeDropItemsEventArgs e) {
            if (null != BeforeDrop) {
                BeforeDrop(this, e);
            }
        }
        /// <summary>
        /// Occurs when control prepares UI container for some data item.
        /// </summary>
        public event EventHandler<PreparingContainerForItemEventArgs> PreparingContainerForItem;
        /// <summary>
        /// Rizes the <see cref="PreparingContainerForItem"/> event.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected virtual void OnPreparingContainerForItem(Windows.UI.Xaml.DependencyObject element, Object item) {
            if (null != PreparingContainerForItem) {
                PreparingContainerForItem(this, new PreparingContainerForItemEventArgs(element, item));
            }
        }
        #endregion

        //----------------------------------------------------------------------
        #region ** fields

        Int32 _lastIndex = -1;  // index of the currently dragged item
        Int32 _currentOverIndex = -1; // index which should be used if we drop immediately
        Int32 _topReorderHintIndex = -1; // index of element which has been moved up (need it to restore item visual state later)
        Int32 _bottomReorderHintIndex = -1; // index of element which has been moved down (need it to restore item visual state later)

        Int32 _lastGroup = -1;  // index of the currently dragged item group
        Int32 _currentOverGroup = -1; // index of the group under the pointer

        #endregion

        //----------------------------------------------------------------------
        #region ** ctor & initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="GridViewExControl"/> control.
        /// </summary>
        public GridViewExControl() {
            this.DefaultStyleKey = typeof(GridViewExControl);
            this.DragItemsStarting += this.GridViewExControl_DragItemsStarting;
        }

        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();
            this._NewGroupPlaceholderFirst = this.GetTemplateChild(NewGroupPlaceholderFirstName) as FrameworkElement;
            this._NewGroupPlaceholderLast = this.GetTemplateChild(NewGroupPlaceholderLastName) as FrameworkElement;
        }
        #endregion

        //----------------------------------------------------------------------
        #region ** overrides
        // set VariableSizedWrapGrid.ColumnSpan and RowSpan properties on element if they are set on item.
        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, Object item) {
            element.SetValue(ContentControl.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch);
            element.SetValue(ContentControl.VerticalContentAlignmentProperty, VerticalAlignment.Stretch);

            var el = item as UIElement;
            if (el != null) {
                Int32 colSpan = Windows.UI.Xaml.Controls.VariableSizedWrapGrid.GetColumnSpan(el);
                Int32 rowSpan = Windows.UI.Xaml.Controls.VariableSizedWrapGrid.GetRowSpan(el);
                if (rowSpan > 1) {
                    // only set it if it has non-defaul value
                    element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.RowSpanProperty, rowSpan);
                }
                if (colSpan > 1) {
                    // only set it if it has non-defaul value
                    element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.ColumnSpanProperty, colSpan);
                }
            }
            this.OnPreparingContainerForItem(element, item);
            base.PrepareContainerForItemOverride(element, item);
        }
        #endregion

        //----------------------------------------------------------------------
        #region ** protected
        /// <summary>
        /// Stores dragged items into DragEventArgs.Data.Properties["Items"] value.
        /// Override this method to set custom drag data if you need to.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDragStarting(DragItemsStartingEventArgs e) {
            e.Data.RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            e.Data.Properties.Add("Items", e.Items);

            // set some custom drag data as below
            // e.Data.SetText(_lastIndex.ToString());
        }

        /// <summary>
        /// Handles drag&drop for cases when it is not supported by the Windows.UI.Xaml.Controls.GridView control (for example, for grouped GridView).
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnDrop(DragEventArgs e) {
            var items = (IList<Object>)e.Data.GetView().Properties["Items"];
            Object item = (items != null && items.Count > 0) ? items[0] : this.Items[this._lastIndex];

            // read custom drag data as below if they have been set in OnDragStarting
            // string text = await e.Data.GetView().GetTextAsync();

            Int32 newIndex = this.GetDragOverIndex(e);
            if (newIndex >= 0) {
                var view = this.ItemsSource as ICollectionView;
                if (view != null && view.CollectionGroups != null && view.CollectionGroups.Count > 0) {
                    // get new group index
                    var root = this.Root;

                    Point position = this.TransformToVisual(root).TransformPoint(e.GetPosition(this));
                    Int32 newGroupIndex = this._currentOverGroup;
                    Boolean requestNewGroupCreation = false;
                    Int32 groupsCount = view.CollectionGroups.Count;
                    // check items directly under the pointer
                    foreach (var element in VisualTreeHelper.FindElementsInHostCoordinates(position, root)) {
                        if (element == this._NewGroupPlaceholderFirst) {
                            newGroupIndex = 0;
                            requestNewGroupCreation = true;
                            break;
                        }
                        if (element == this._NewGroupPlaceholderLast) {
                            newGroupIndex = groupsCount;
                            requestNewGroupCreation = true;
                            break;
                        } else if (element is FrameworkElement && ((FrameworkElement)element).Name.ToLower() == "newgroupplaceholdercontrol") {
                            newGroupIndex = this._currentOverGroup + 1;
                            requestNewGroupCreation = true;
                            break;
                        }
                    }
                    if (!requestNewGroupCreation && this._lastGroup == newGroupIndex && newIndex > this._lastIndex) {
                        // adjust newIndex if me move item forward
                        newIndex--;
                    }
                    var args = new BeforeDropItemsEventArgs(item, this._lastIndex, newIndex, this._lastGroup, newGroupIndex, requestNewGroupCreation, e);
                    this.OnBeforeDrop(args);

                    if (!args.Cancel) {
                        view = this.ItemsSource as ICollectionView;
                        if (groupsCount != view.CollectionGroups.Count && newGroupIndex == 0) {
                            this._lastGroup++;
                        }
                        var oldGroup = (ICollectionViewGroup)view.CollectionGroups[this._lastGroup];
                        if (newGroupIndex < view.CollectionGroups.Count) {
                            var newGroup = (ICollectionViewGroup)view.CollectionGroups[newGroupIndex];
                            if (newGroup != null) {
                                // get index in the new group to insert
                                newIndex = newGroup.GroupItems.IndexOf(this.Items[newIndex]);

                                // todo: fire event, something like BeforeDrop? Cancellable, with information, 
                                // so that user can update item properties which depend on item group
                                if (oldGroup != null) {
                                    oldGroup.GroupItems.Remove(item);
                                }
                                if (newIndex >= 0) {
                                    newGroup.GroupItems.Insert(newIndex, item);
                                } else {
                                    // insert after the last item in the group
                                    newGroup.GroupItems.Add(item);
                                }
                            }
                        }
                    }
                } else if (newIndex != this._lastIndex) {
                    if (newIndex > this._lastIndex) {
                        // adjust newIndex if me move item forward
                        newIndex--;
                    }
                    var args = new BeforeDropItemsEventArgs(item, this._lastIndex, newIndex, e);
                    this.OnBeforeDrop(args);
                    if (!args.Cancel) {
                        var source = this.ItemsSource as System.Collections.IList;
                        if (source != null) {
                            source.RemoveAt(this._lastIndex);
                            source.Insert(newIndex, item);
                        } else {
                            this.Items.RemoveAt(this._lastIndex);
                            this.Items.Insert(newIndex, item);
                        }
                    }
                }
            }
            this._lastIndex = -1;
            this._currentOverIndex = -1;
            this._lastGroup = -1;
            this._currentOverGroup = -1;

            base.OnDrop(e);
        }

        /// <summary>
        /// Shows reoder hints while custom dragging.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragOver(DragEventArgs e) {
            /* set ReorderHintStates for underlying items
                * possible ReorderHintStates:
                    - "NoReorderHint"
                    - "BottomReorderHint"
                    - "TopReorderHint"
                    - "RightReorderHint"
                    - "LeftReorderHint"
                */
            Int32 newIndex = this.GetDragOverIndex(e);
            if (newIndex >= 0) {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
                if (this._currentOverIndex != newIndex) {
                    this._currentOverIndex = newIndex;
                    if (this._topReorderHintIndex != -1) {
                        this.GoItemToState(this._topReorderHintIndex, "NoReorderHint", true);
                        this._topReorderHintIndex = -1;
                    }
                    if (this._bottomReorderHintIndex != -1) {
                        this.GoItemToState(this._bottomReorderHintIndex, "NoReorderHint", true);
                        this._bottomReorderHintIndex = -1;
                    }
                    if (newIndex > 0) {
                        this._topReorderHintIndex = newIndex - 1;
                    }
                    if (newIndex < this.Items.Count) {
                        this._bottomReorderHintIndex = newIndex;
                    }
                    if (this.IsGrouping && this._currentOverGroup >= 0) {
                        Int32 topHintGroup = this.GetGroupForIndex(this._topReorderHintIndex);
                        if (topHintGroup != this._currentOverGroup) {
                            this._topReorderHintIndex = -1;
                        }
                        Int32 bottomHintGroup = this.GetGroupForIndex(this._bottomReorderHintIndex);
                        if (bottomHintGroup != this._currentOverGroup) {
                            this._bottomReorderHintIndex = -1;
                        }
                    }
                    if (this._topReorderHintIndex >= 0) {
                        this.GoItemToState(this._topReorderHintIndex, "TopReorderHint", true);
                    }
                    if (this._bottomReorderHintIndex >= 0) {
                        this.GoItemToState(this._bottomReorderHintIndex, "BottomReorderHint", true);
                    }
                }
            }
            base.OnDragOver(e);
        }
        #endregion

        //----------------------------------------------------------------------
        #region ** private
        private void GridViewExControl_DragItemsStarting(Object sender, DragItemsStartingEventArgs e) {
            this._currentOverIndex = -1;
            this._topReorderHintIndex = -1;
            this._bottomReorderHintIndex = -1;
            this._lastGroup = -1;
            this._currentOverGroup = -1;
            Object item = e.Items[0];
            this._lastIndex = this.IndexFromContainer(this.ContainerFromItem(item));
            this._lastGroup = this.GetItemGroup(item);
            this.OnDragStarting(e);
        }

        private Int32 GetDragOverIndex(DragEventArgs e) {
            var root = this.Root as FrameworkElement;

            Point position = this.TransformToVisual(root).TransformPoint(e.GetPosition(this));

            Int32 newIndex = -1;

            // check items directly under the pointer
            foreach (var element in VisualTreeHelper.FindElementsInHostCoordinates(position, root)) {
                // assume horizontal orientation
                var container = element as ContentControl;
                if (container == null) {
                    continue;
                }

                Int32 tempIndex = this.IndexFromContainer(container);
                if (tempIndex >= 0) {
                    this._currentOverGroup = this.GetItemGroup(container.Content);
                    // we only need GridViewItems belonging to this GridView control
                    // if we found one - we done
                    newIndex = tempIndex;
                    // adjust index depending on pointer position
                    Point center = container.TransformToVisual(root).TransformPoint(new Point(container.ActualWidth / 2, container.ActualHeight / 2));
                    if (position.Y > center.Y) {
                        newIndex++;
                    }
                    break;
                }
            }
            if (newIndex < 0) {
                // if we haven't found item under the pointer, check items in the rectangle to the left from the pointer position
                foreach (var element in GetIntersectingItems(position, root)) {
                    // assume horizontal orientation
                    var container = element as ContentControl;
                    if (container == null) {
                        continue;
                    }

                    Int32 tempIndex = this.IndexFromContainer(container);
                    if (tempIndex < 0) {
                        // we only need GridViewItems belonging to this GridView control
                        // so skip all elements which are not
                        continue;
                    }
                    Rect bounds = container.TransformToVisual(root).TransformBounds(new Rect(0, 0, container.ActualWidth, container.ActualHeight));

                    if (bounds.Left <= position.X && bounds.Top <= position.Y && tempIndex > newIndex) {
                        this._currentOverGroup = this.GetItemGroup(container.Content);
                        newIndex = tempIndex;
                        // adjust index depending on pointer position
                        if (position.Y > bounds.Top + container.ActualHeight / 2) {
                            newIndex++;
                        }
                        if (bounds.Right > position.X && bounds.Bottom > position.Y) {
                            break;
                        }
                    }
                }
            }
            if (newIndex < 0) {
                newIndex = 0;
            }
            if (newIndex >= this.Items.Count) {
                newIndex = this.Items.Count - 1;
            }
            return newIndex;
        }

        /// <summary>
        /// returns all items in the rectangle with x=0, y=0, width=intersectingPoint.X, height=root.ActualHeight.
        /// </summary>
        /// <param name="intersectingPoint"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        private static IEnumerable<UIElement> GetIntersectingItems(Point intersectingPoint, FrameworkElement root) {
            var rect = new Rect(0, 0, intersectingPoint.X, root.ActualHeight);
            return VisualTreeHelper.FindElementsInHostCoordinates(rect, root);
        }

        private void GoItemToState(Int32 index, String state, Boolean useTransitions) {
            if (index >= 0) {
                var control = this.ContainerFromIndex(index) as Control;
                if (control != null) {
                    VisualStateManager.GoToState(control, state, useTransitions);
                }
            }
        }

        private Int32 GetGroupForIndex(Int32 index) {
            if (index < 0) {
                return index;
            }
            return this.GetItemGroup(this.Items[index]);
        }
        private Int32 GetItemGroup(Object item) {
            var view = this.ItemsSource as ICollectionView;
            if (view != null && view.CollectionGroups != null) {
                foreach (ICollectionViewGroup gr in view.CollectionGroups) {
                    if (gr.Group == item || gr.GroupItems.IndexOf(item) >= 0) {
                        return view.CollectionGroups.IndexOf(gr);
                    }
                }
            }
            return -1;
        }
        #endregion

    }

    /// <summary>
    /// Provides data for the <see cref="GridViewExControl.BeforeDrop"/> event.
    /// </summary>
    public sealed class BeforeDropItemsEventArgs : System.ComponentModel.CancelEventArgs {
        internal BeforeDropItemsEventArgs(Object item, Int32 oldIndex, Int32 newIndex, DragEventArgs dragEventArgs)
            : this(item, oldIndex, newIndex, -1, -1, false, dragEventArgs) {
        }
        internal BeforeDropItemsEventArgs(Object item, Int32 oldIndex, Int32 newIndex,
            Int32 oldGroupIndex, Int32 newGroupIndex, Boolean requestCreateNewGroup, DragEventArgs dragEventArgs)
            : base() {
            this.RequestCreateNewGroup = requestCreateNewGroup;
            this.OldGroupIndex = oldGroupIndex;
            this.NewGroupIndex = newGroupIndex;
            this.OldIndex = oldIndex;
            this.NewIndex = newIndex;
            this.Item = item;
        }

        /// <summary>
        /// Gets the item which is beeing dragged.
        /// </summary>
        public Object Item {
            get;
            private set;
        }
        /// <summary>
        /// Gets the current item index in the underlying data source.
        /// </summary>
        public Int32 OldIndex {
            get;
            private set;
        }
        /// <summary>
        /// Gets the index in the underlying data source where the item will be insertet by the drop operation.
        /// </summary>
        public Int32 NewIndex {
            get;
            private set;
        }
        /// <summary>
        /// Gets the <see cref="Boolean"/> value determining whether end-user actions requested creation of the new group in the underlying data source.
        /// This property only makes sense if GridViewExControl.IsGrouping property is true.
        /// </summary>
        /// <remarks>
        /// If this property is true, create the new data group and insert it into the groups collection at the positions, specified by the 
        /// <see cref="BeforeDropItemsEventArgs.NewGroupIndex"/> property value. Then the <see cref="GridViewExControl"/> will insert dragged item
        /// into the newly added group.
        /// </remarks>
        public Boolean RequestCreateNewGroup {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the current item data group index in the underlying data source.
        /// This property only makes sense if GridViewExControl.IsGrouping property is true.
        /// </summary>
        public Int32 OldGroupIndex {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the data group index in the underlying data source where the item will be insertet by the drop operation.
        /// This property only makes sense if GridViewExControl.IsGrouping property is true.
        /// </summary>
        public Int32 NewGroupIndex {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the original <see cref="DragEventArgs"/> data. 
        /// </summary>
        public DragEventArgs DragEventArgs {
            get;
            private set;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="GridViewExControl.PreparingContainerForItem"/> event.
    /// </summary>
    public sealed class PreparingContainerForItemEventArgs : EventArgs {
        internal PreparingContainerForItemEventArgs(Windows.UI.Xaml.DependencyObject element, Object item)
            : base() {
            this.Element = element;
            this.Item = item;
        }

        /// <summary>
        /// Gets the item for which container is beeing prepared.
        /// </summary>
        public Object Item {
            get;
            private set;
        }
        /// <summary>
        /// Gets the container prepared to show in UI.
        /// </summary>
        public Windows.UI.Xaml.DependencyObject Element {
            get;
            private set;
        }
    }
}
