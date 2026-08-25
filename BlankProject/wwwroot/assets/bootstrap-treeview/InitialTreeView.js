var selectedCategories = [];
var InitialFinished = false;

$tree = $("#cats");
$catIds = $("#cat-ids");

function RefreshCatList(TypeId) {
    $.get("/Admin/Categories/AjaxGetCategories?TypeId=" + TypeId, function (data) {
        InitList(data);
        InitialSelectedNode();
    })
}

//ایجاد treeview
function InitList(data) {
    $tree.treeview({
        data: data,
        levels: 0,
        multiSelect: true,
        onNodeSelected: function (event, node) {
            selectedCategories = $tree.treeview('getSelected');
            RefreshChips();

            //select parents
            var temp = node;
            while (temp.parentCategoryId != null) {
                var Parent = $tree.treeview('getParent', temp);
                $tree.treeview('selectNode', Parent);
                temp = Parent;
            }
            RenderSelectedCategories()
            //console.log(JSON.stringify(selectedCategories))
        },
        onNodeUnselected: function (event, node) {
            selectedCategories = $tree.treeview('getSelected');
            RefreshChips();

            //unselect all child
            var Childs = _getChildren(node);
            var deletedItems = [];
            for (var i = 0; i < selectedCategories.length; i++) {
                for (var j = 0; j < Childs.length; j++) {
                    if (selectedCategories[i].id == Childs[j].id) {
                        deletedItems.push(selectedCategories[i]);
                    }
                }
            }

            for (var i = 0; i < deletedItems.length; i++) {
                $tree.treeview('unselectNode', deletedItems[i]);
            }
            RenderSelectedCategories()
            //console.log(JSON.stringify(selectedCategories))
        }
    });
}




//انتخاب دسته بندی های یک پست
function InitialSelectedNode() {
    if (PostCategoriesId.length > 0) {
        var UnselectedNode = $tree.treeview('getUnselected');
        var SelectedNode = [];
        for (var i = 0; i < UnselectedNode.length; i++) {
            for (var j = 0; j < PostCategoriesId.length; j++) {
                if (UnselectedNode[i].id == PostCategoriesId[j]) {
                    $tree.treeview('selectNode', UnselectedNode[i]);
                    if (UnselectedNode[i].parentCategoryId != null) {
                        var Parent = $tree.treeview('getParent', UnselectedNode[i]);
                        $tree.treeview('expandNode', Parent);
                    }
                }
            }
        }
        RenderSelectedCategories();
    }
    InitialFinished = true;
}




//قرار دادن ایدی دسته بندی های انتخاب شده درون اینپوت
function RenderSelectedCategories() {
    //for (var i = 0; i < selectedCategories.length; i++) {
    //    console.log(selectedCategories[i].text);
    //}
    //console.log('------------------------------------------');

    $catIds.val("");
    var str = "";
    $(selectedCategories).each(function (i) {
        str += this.id;
        if (i != selectedCategories.length - 1)
            str += ","
    });
    $catIds.val(str)
    //LoadCategoryProperties(str);
};



//گرفتن همه فرزند ها و نوه های یک نُود
function _getChildren(node) {
    if (node.nodes === undefined) return [];
    if (!node.nodes) return [];
    var childrenNodes = node.nodes;
    if (node.nodes) {
        node.nodes.forEach(function (n) {
            childrenNodes = childrenNodes.concat(_getChildren(n));
        });
    }
    return childrenNodes;
}



//حذف چیپ و برداشتن انتخاب نُود مورد نظر
function RemoveChip(el, id) {
    var SelectedNode = $tree.treeview('getSelected');
    for (var i = 0; i < SelectedNode.length; i++) {
        if (SelectedNode[i].id == id)
            $tree.treeview('unselectNode', SelectedNode[i]);
    }
    RefreshChips()
    $(el).parents(".pmd-chip").remove()
}



//ساختن چیپ ها با توجه به ایتم های انتخاب شده
function RefreshChips() {
    $(".pmd-chip").remove();
    var SelectedNode = $tree.treeview('getSelected');
    for (var i = 0; i < SelectedNode.length; i++) {
        $("#chip-holder").append(CreateChip(SelectedNode[i]));
    }
}


//ساختن یک چیپ
function CreateChip(node) {
    return '<div class="pmd-chip">'
        + node.text
        + '<a class="pmd-chip-action" onclick="RemoveChip(this,' + node.id + ');" data-catId="' + node.id + '"><i class="material-icons">close</i></a>'
        + '</div>'
}




