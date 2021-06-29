/*! SearchBuilder 1.0.1
 * ©2020 SpryMedia Ltd - datatables.net/license/mit
 */
(function () {
    'use strict';

    (function (factory) {
        if (typeof define === 'function' && define.amd) {
            // AMD
            define(['jquery', 'datatables.net-se', 'datatables.net-searchbuilder'], function ($) {
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
                    $ = require('datatables.net-se')(root, $).$;
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
            clearAll: 'ui button dtsb-clearAll'
        });
        $.extend(true, DataTable.Group.classes, {
            add: 'ui button dtsb-add',
            clearGroup: 'ui button dtsb-clearGroup',
            logic: 'ui button dtsb-logic'
        });
        $.extend(true, DataTable.Criteria.classes, {
            condition: 'ui selection dropdown dtsb-condition',
            data: 'ui selection dropdown dtsb-data',
            "delete": 'ui button dtsb-delete',
            left: 'ui button dtsb-left',
            right: 'ui button dtsb-right',
            value: 'ui selection dropdown dtsb-value'
        });
        return DataTable.searchPanes;
    }));

}());
