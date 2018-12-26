'use strict';
var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function (req, res) {
    let tms = [{ value: '1141', name: '2008年' },{ value: '1142', name: '2010年' },{ value: '1143', name: '2012年' }, { value: '1144', name: '2016年' }];
    res.render('index', { title: '多时相影像切片服务：一个服务接口，支持回溯历史影像。' ,tms:tms});
});

module.exports = router;
