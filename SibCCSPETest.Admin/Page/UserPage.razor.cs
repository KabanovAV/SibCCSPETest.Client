using Microsoft.AspNetCore.Components;
using SibCCSPETest.Data;
using SibCCSPETest.ServiceBase;
using SibCCSPETest.Shared.Components;

namespace SibCCSPETest.Admin.Page
{
    public partial class UserPage
    {
        [Inject]
        public IAPIService? ServiceAPI { get; set; }

        private NexusTableGrid<UserDTO>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Single;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        private List<UserDTO> Items = [];
        private UserDTO? Data = new();

        public bool IsCrud => NexusTable != null
            && (NexusTable.InsertItem.Count > 0 || NexusTable.EditedItem.Count > 0);
        public bool IsSaveCancel => !IsCrud;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            var u = await ServiceAPI!.UserService.GetAllUser();
            Items = u.ToList();
        }

        public async Task InserRow()
            => await NexusTable!.InsertRow(new UserDTO());

        public void EditRow()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
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

        public async Task Add(UserDTO item)
        {
            Data = await ServiceAPI!.UserService.AddUser(item);
            if (Data != null)
            {
                NexusTable!.Data.Add(Data);
                await NexusTable.SelectRow(Data);                
            }
        }

        public async Task Update(UserDTO item)
        {
            await ServiceAPI!.UserService.UpdateUser(item);
            NexusTable.CancelEditRow(Data);
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                Data = NexusTable.SelectedRows.First();
                await ServiceAPI!.UserService.DeleteUser(Data.Id);
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
