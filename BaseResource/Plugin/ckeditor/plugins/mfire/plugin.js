(function () {
    CKEDITOR.plugins.add('mfire',
    {
        init: function (editor) {
            //----------------布局定义----------------------start
            editor.ui.addButton('LayoutSelect',
            {
                label: '布局定义',
                icon: CKEDITOR.plugins.get("mfire").path + "images/layoutType.png",
                command: 'LayoutSelectCommand'
            });
            // Add the link and unlink buttons.
            editor.addCommand('LayoutSelectCommand', new CKEDITOR.dialogCommand('openMFireDialog'));
            //CKEDITOR.dialog.add( 'link’, this.path + 'dialogs/link.js’ );  
            //dialog也可用抽離出去變一個js，不過這裡我直接寫在下面
            CKEDITOR.dialog.add('openMFireDialog', function (editor) {
                return {
                    title: '布局定义',
                    minWidth: 200,
                    minHeight: 100,
                    contents: [
                        {
                            id: 'code',
                            label: 'code',
                            title: 'code',
                            elements:              //elements是定義dialog內部的元件，除了下面用到的select跟textarea之外
                                [                  //還有像radio或是file之類的可以選擇
                                {
                                    type: 'select',
                                    label: '布局选择',
                                    id: 'layoutType',
                                    //required: true,
                                    'default': '1',
                                    items: [['单列式', '1'], ['双列式', '2'], ['三列式', '3']]
                                }

                                ]
                        }
                    ],
                    onOk: function () {
                        var layoutType = this.getValueOf('code', 'layoutType');
                        if (editor.LayoutSelected) {
                            editor.LayoutSelected(layoutType);
                        }
                    }
                };
            });
            //----------------布局定义----------------------end

            //------------------预览------------------------start
            editor.ui.addButton('LayoutPreview',
                {
                    label: '预览',
                    icon: CKEDITOR.plugins.get("mfire").path + "images/preview.png",
                    command: 'LayoutPreviewCommand'
                });
            // Add the link and unlink buttons.
            editor.addCommand('LayoutPreviewCommand', {
                exec: function (editor) {
                    if (editor.LayoutPreview) {
                        editor.LayoutPreview();
                    }
                }
            });
            //------------------预览------------------------end
            //------------------保存------------------------start
            editor.ui.addButton('SaveLayout',
                {
                    label: '保存布局',
                    icon: CKEDITOR.plugins.get("mfire").path + "images/saveLayout.png",
                    command: 'SaveLayoutCommand'
                });
            // Add the link and unlink buttons.
            editor.addCommand('SaveLayoutCommand', {
                exec: function (editor) {
                    if (editor.SaveLayout) {
                        editor.SaveLayout(editor);
                    }
                }
            });
            //------------------保存------------------------end
        }
    });
})();