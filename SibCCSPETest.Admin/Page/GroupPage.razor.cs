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
        [Inject]
        public INexusNotificationService? NotificationService { get; set; }
        [Inject]
        public INexusDialogService? DialogService { get; set; }

        private NexusTableGrid<GroupDTO>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Single;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        private List<GroupDTO>? Items;
        private IEnumerable<SelectItem>? SpecializationSelectItems;

        public bool IsCrud => NexusTable != null
            && (NexusTable.InsertItem.Count > 0 || NexusTable.EditedItem.Count > 0);
        public bool IsSelected => IsCrud || !NexusTable.IsRowsSelected;
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
                    var data = NexusTable.SelectedRows.First();
                    NexusTable.EditRow(data);
                }
            }
        }

        public async Task Save()
        {
            if (NexusTable!.InsertItem.Count == 0 && NexusTable!.EditedItem.Count > 0)
            {
                var data = NexusTable!.EditedItem.First();
                await Update(data);
            }
            else
            {
                var data = NexusTable!.InsertItem.First();
                await Add(data);
            }
            await NexusTable.Reload();
        }

        public async Task Add(GroupDTO item)
        {
            var data = await ServiceAPI!.GroupService.AddGroup(item);
            if (data != null)
            {
                NexusTable!.Data.Add(data);
                await NexusTable.SelectRow(data);
                NotificationService.ShowSuccess("Группа добавлена", "Успех");
            }
        }

        public async Task Update(GroupDTO item)
        {
            var data = await ServiceAPI!.GroupService.UpdateGroup(item);
            if (data != null)
            {
                var index = NexusTable!.Data.FindIndex(s => s.Id == data.Id);
                if (index >= 0)
                    NexusTable.Data[index] = data;
                await NexusTable.SelectRow(data);
                await NexusTable.CancelEditRow(data);
                NotificationService.ShowSuccess("Группа изменена", "Успех");
            }
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                var data = NexusTable.SelectedRows.First();
                var settings = new NexusDialogSetting("Удаление группы", $"Вы уверены, что хотите удалить \"{data.Title}\" группу?", "Отменить", "Удалить");
                var result = await DialogService.Show(settings);
                if (result?.Canceled == false)
                {
                    await ServiceAPI!.GroupService.DeleteGroup(data.Id);
                    NexusTable.RemoveRow(data);
                    NotificationService.ShowSuccess("Группа удалена", "Успех");
                }
            }
        }

        public async Task Cancel()
        {
            var data = NexusTable!.SelectedRows.First();
            await NexusTable.CancelEditRow(data);
        }

        //public void OnSelecChange(ChangeEventArgs args, GroupDTO item)
        //{
        //    var result = Int32.Parse(args.Value.ToString());
        //    item.SpecializationId = result;
        //    item.SpecializationTitle = SpecializationSelectItems.First(s => s.Value == result).Text;
        //}
    }
}
