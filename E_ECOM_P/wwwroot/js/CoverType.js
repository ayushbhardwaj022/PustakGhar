var dataTable; /* global variable dataTable to store the DataTable instance*/

$(document).ready(function () {
    loadDatatable(); /* Calls the loadDatatable() function to initialize the table*/

});
function loadDatatable() { /*Defines the function that sets up the DataTable*/
    dataTable = $('#tbldata').DataTable({  /*   datalibrary class*/
        "pageLength": 3, // sirf 2 entries show hongi ek page par coz of pagelength
        "lengthMenu": [3, 4, 6, 8], //ye entry per page dropdown list ke liye
          "lengthChange":true,                //agar ise hum false kr denge toh   pages wali dropdown show nhi krta                     


        "ajax": {
            "url": "/Admin/CoverType/GetAll"   /*Area controller actionmenthod*/
        },
        //"columns": [
        //    { "data": "name", "width": "70%" },  here name and actions are defined under one column where covertype names are 70% on screen whereas 30% on action buttons
        //    {
        //        "data": "id",
        //        "render": function (data) {
        //            return `
        //        <div class="text-center">
        //        <a href="/Admin/CoverType/Upsert/${data}" class="btn btn-info">   hyper link tag i.e,  used to move from one page to another
        //        <i class="fas fa-edit"></i>
        //        </a>
                                     
        //        <a class="btn btn-danger" onclick="Delete('/Admin/CoverType/Delete/${data}')">
        //        <i class="fas fa-trash-alt"></i>
        //        </a>
        //        </div>
        //        `;
        //        }

        //    }
        "columns": [             //here every colunm name divided differently firstly 15%screensize for edit then covertype name 70% lastly, for delete 15%
    {
            "data": "id",
                "render": function (data) {             /*first action=edit, edit pr click krne par upsert par jayega coz href(hyperlink)*/  /*here $ access data by id*/     /*area/controller/action loaction*/
                return `
                <a href="/Admin/CoverType/Upsert/${data}" class="btn btn-info">    
                    <i class="fas fa-edit"></i>    
                </a>
            `;
            },
            "width": "15%"
        },
            { "data": "name", "width": "70%" },  //covertype names screensize//        /*area/controller/action*/ loaction
        {
            "data": "id",                     /*delete function 15% screensize*/
            "render": function (data) {
                return `
                <a class="btn btn-danger" onclick="Delete('/Admin/CoverType/Delete/${data}')">           
                    <i class="fas fa-trash-alt"></i>
                </a>
            `;
            },
            "width": "15%"
        }
]
    });

}                  /*(ab hum cover.js ko index me call krnge*/
function Delete(url) {
    /*alert(url);*/
    swal({
        title: "Want To Delete Data?",
        text: "Delete Information!!!!!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });

}