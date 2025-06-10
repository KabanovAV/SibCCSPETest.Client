using Microsoft.AspNetCore.Components;

namespace SibCCSPETest.Shared.Components
{
    public partial class NexusTableGrid<TItem> : ComponentBase
    {
        [Parameter, EditorRequired]
        public List<TItem> Data { get; set; }
        [Parameter]
        public string? EmptyMessage { get; set; }
        [Parameter]
        public string? CssClass { get; set; }
        [Parameter]
        public string? CssStyle { get; set; }
        [Parameter]
        public RenderFragment? ToolTemplate { get; set; }
        [Parameter]
        public RenderFragment? Columns { get; set; }
        [Parameter]
        public RenderFragment<TItem>? EditColumns { get; set; }
        [Parameter]
        public NexusTableGridSelectionMode SelectionMode { get; set; }
        [Parameter]
        public NexusTableGridEditMode EditMode { get; set; }
        [Parameter]
        public EventCallback OnSelectRow { get; set; }

        private bool IsTableEmpty;
        private List<NexusTableGridColumn<TItem>> _items = [];
        public List<NexusTableGridColumn<TItem>> Items
        {
            get => _items;
            set => _items = value;
        }

        public List<TItem> SelectedRows { get; set; } = [];
        public List<TItem> InsertItem { get; set; } = [];
        public List<TItem> EditedItem { get; set; } = [];

        private bool IsInsertMode { get; set; }
        private bool IsEditMode { get; set; }
        private bool IsHasItems => Items.Count > 0 || InsertItem.Count > 0;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            IsTableEmpty = _items.All(x => !x.Visible);
        }

        public void AddItem(NexusTableGridColumn<TItem> item)
        {
            if (_items.IndexOf(item) == -1)
            {
                _items.Add(item);
                Refresh();
            }
        }

        public async Task SelectRow(TItem item)
        {
            if (SelectionMode == NexusTableGridSelectionMode.Single)
            {
                SelectedRows.Clear();
                SelectedRows?.Add(item);
            }
            else
            {
                if (SelectedRows.Contains(item))
                    SelectedRows.Remove(item);
                else
                    SelectedRows.Add(item);
            }
            await OnSelectRow.InvokeAsync();
        }

        public bool IsRowsSelected()
            => SelectedRows.Count != 0;

        public bool IsRowSelected(TItem item)
            => SelectedRows.Contains(item);

        public async Task InsertRow(TItem item)
        {
            InsertItem?.Add(item);
            await SelectRow(item);
            EditRow(item);
            IsInsertMode = true;
        }

        public void EditRow(TItem item)
        {
            if (EditMode == NexusTableGridEditMode.Single)
            {
                EditedItem.Clear();
                EditedItem.Add(item);
            }
            else
            {
                if (!EditedItem.Contains(item))
                    EditedItem.Add(item);
            }
            IsEditMode = true;
        }

        public void RemoveRow(TItem item)
        {
            Data.Remove(item);
            SelectedRows.Remove(item);
        }

        public void CancelEditRow(TItem item)
        {
            if (IsInsertMode)
            {
                Data.Remove(item);
                SelectedRows.Remove(item);
            }
            ResetRow(item);
        }

        public void ResetRow(TItem item)
        {
            if (EditedItem.Contains(item))
            {
                EditedItem.Remove(item);
                IsEditMode = EditedItem.Count == 0 ? false : true;
            }
            if (InsertItem.Contains(item))
            {
                InsertItem.Remove(item);
                IsInsertMode = InsertItem.Count == 0 ? false : true;
            }
        }

        public async Task Reload()
        {
            if (IsInsertMode)
            {
                foreach (var item in InsertItem)
                    Data.Remove(item);
            }
            else if (IsEditMode)
            {
                foreach (var item in EditedItem)
                    Data.Remove(item);
            }
            InsertItem.Clear();
            EditedItem.Clear();
            IsInsertMode = false;
            IsEditMode = false;
            await InvokeAsync(StateHasChanged);
        }

        public void Refresh() => StateHasChanged();
        public async Task RefreshAsync() => await InvokeAsync(StateHasChanged);
    }
}
