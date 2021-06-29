/*! SearchBuilder 1.0.1
 * ©2020 SpryMedia Ltd - datatables.net/license/mit
 */
(function () {
    'use strict';

    (function (factory) {
        if (typeof define === 'function' && define.amd) {
            // AMD
            define(['jquery', 'datatables.net-zf', 'datatables.net-searchbuilder'], function ($) {
                return factory($, window, document);
            });
        }
        else if (typeof exports === 'object') {
            // CommonJS
            module.exports = function (root, $) {
                if (!root) {
                    root = window;
                }
                if (!$ || !$.fn.dataTable) {
                    $ = require('datatables.net-zf')(root, $).$;
                }
                if (!$.fn.dataTable.searchBuilder) {
                    require('datatables.net-searchbuilder')(root, $);
                }
                return factory($, root, root.document);
            };
        }
        else {
            // Browser
            factory(jQuery, window, document);
        }
    }(function ($, window, document) {
        var DataTable = $.fn.dataTable;
        $.extend(true, DataTable.SearchBuilder.classes, {
            clearAll: 'button secondary dtsb-clearAll'
        });
        $.extend(true, DataTable.Group.classes, {
            add: 'button secondary dtsb-add',
            clearGroup: 'button secondary dtsb-clearGroup',
            logic: 'button secondary dtsb-logic'
        });
        $.extend(true, DataTable.Criteria.classes, {
            condition: 'form-control dtsb-condition',
            data: 'form-control dtsb-data',
            "delete": 'button secondary dtsb-delete',
            left: 'button secondary dtsb-left',
            right: 'button secondary dtsb-right',
            value: 'form-control dtsb-value'
        });
        return DataTable.searchPanes;
    }));

}());
