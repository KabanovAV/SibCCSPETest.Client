using Microsoft.AspNetCore.Components;
using SibCCSPETest.Data;
using SibCCSPETest.ServiceBase;
using SibCCSPETest.Shared.Components;

namespace SibCCSPETest.Admin.Page
{
    public partial class GroupPage
    {
        [Inject]
        public IAPIService? ServiceAPI { get; set; }

        private NexusTableGrid<GroupDTO>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Single;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        private List<GroupDTO>? Items;
        private GroupDTO? Data = new();
        private IEnumerable<SelectItem>? SpecializationSelectItems;

        public bool IsCrud => NexusTable != null
            && (NexusTable.InsertItem.Count > 0 || NexusTable.EditedItem.Count > 0);
        public bool IsSelected => IsCrud || !NexusTable.IsRowsSelected();
        public bool IsSaveCancel => !IsCrud;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task LoadData()
        {
            var g = await ServiceAPI!.GroupService.GetAllGroup();
            Items = g.ToList();
        }

        public async Task InserRow()
        {
            SpecializationSelectItems = await ServiceAPI.SpecializationService.GetSpecializationSelect();
            await NexusTable!.InsertRow(new GroupDTO { Begin = DateTime.Now, End = DateTime.Now });
        }


        public async Task EditRow()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                SpecializationSelectItems = await ServiceAPI.SpecializationService.GetSpecializationSelect();
                if (EditMode == NexusTableGridEditMode.Multiple
                && SelectMode == NexusTableGridSelectionMode.Multiple)
                {
                    foreach (var selectRow in NexusTable.SelectedRows)
                    {
                        NexusTable.EditRow(selectRow);
                    }
                }
                else
                {
                    Data = NexusTable.SelectedRows.First();
                    NexusTable.EditRow(Data);
                }
            }
        }

        public async Task Save()
        {
            Data = NexusTable!.EditedItem.First();
            if (Data.Id != 0)
                await Update(Data);
            else
                await Add(Data);
            await NexusTable.Reload();
        }

        public async Task Add(GroupDTO item)
        {
            Data = await ServiceAPI!.GroupService.AddGroup(item);
            if (Data != null)
            {
                NexusTable!.Data.Add(Data);
                await NexusTable.SelectRow(Data);
            }
        }

        public async Task Update(GroupDTO item)
        {
            await ServiceAPI!.GroupService.UpdateGroup(item);
            NexusTable.CancelEditRow(Data);
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                Data = NexusTable.SelectedRows.First();
                await ServiceAPI!.GroupService.DeleteGroup(Data.Id);
                NexusTable.RemoveRow(Data);
            }
        }

        public void Cancel()
        {
            Data = NexusTable!.SelectedRows.First();
            NexusTable.CancelEditRow(Data);
        }
    }
}
