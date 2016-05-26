﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Models;
using System.Transactions;

namespace WMS.Controllers
{
    /// <summary>
    /// 残损捡货单模块
    /// </summary>
    public class CsRetrieveController : SsnController
    {

        /// <summary>
        /// 得到当前日期的拣货单
        /// </summary>
        /// <param name="bdat">开始时间</param>
        /// <param name="edat">结束时间</param>
        /// <param name="lnkbllid">连接订单（为空/206分店拣货单、501外销单）</param>
        /// <returns></returns>
        [PWR(Pwrid = WMSConst.WMS_BACK_拣货查询, pwrdes = "拣货查询")]
        public ActionResult GetRetrieveBll(String bdat, String edat, String lnkbllid)
        {
            if (lnkbllid == null)
            {
                lnkbllid = "206";
            }

            if (bdat == null)
            {
                bdat = GetCurrentDay();
            }
            if (edat == null)
            {
                edat = GetNextDay();
            }

            //分店拣货单
            var qry = from e in WmsDc.wms_cang
                      where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                      && e.lnkbllid == lnkbllid
                      && (e.savdptid == LoginInfo.DefCsSavdptid)
                      && e.mkedat.CompareTo(bdat) >= 0 && e.mkedat.CompareTo(edat) < 0
                      && qus.Contains(e.qu.Trim())
                      && !(from ee in WmsDc.wms_bzcnv where ee.lnkbllid == "207" && ee.wmsbllid == "103" && ee.wmsno == e.wmsno select ee).Any()
                      select new
                      {
                          e.bllid,
                          e.brief,
                          e.chkdat,
                          e.chkflg,
                          e.ckr,
                          e.lnkbllid,
                          e.lnkbocidat,
                          e.lnkbocino,
                          e.lnkbrief,
                          e.lnkno,
                          e.mkedat,
                          e.mkr,
                          e.opr,
                          e.prvid,
                          e.qu,
                          e.rcvdptid,
                          e.savdptid,
                          e.times,
                          e.wmsno,
                          prvdes = ""
                      };
            if (lnkbllid == "501")
            {
                //外销单
                qry = from e in WmsDc.wms_cang
                      join e1 in WmsDc.cus on e.prvid equals e1.cusid
                      where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                      && e.lnkbllid == lnkbllid
                       && (e.savdptid == LoginInfo.DefCsSavdptid)
                       && e.mkedat.CompareTo(bdat) >= 0 && e.mkedat.CompareTo(edat) < 0
                      && qus.Contains(e.qu.Trim())
                      select new
                      {
                          e.bllid,
                          e.brief,
                          e.chkdat,
                          e.chkflg,
                          e.ckr,
                          e.lnkbllid,
                          e.lnkbocidat,
                          e.lnkbocino,
                          e.lnkbrief,
                          e.lnkno,
                          e.mkedat,
                          e.mkr,
                          e.opr,
                          e.prvid,
                          e.qu,
                          e.rcvdptid,
                          e.savdptid,
                          e.times,
                          e.wmsno,
                          prvdes = e1.cusdes
                      };
            }

            var arrqry = qry.ToArray();
            if (arrqry.Length <= 0)
            {
                return RNoData("未找到当前日期的拣货单");
            }
            return RSucc("成功", arrqry);
        }

        /// <summary>
        /// 得到拣货单明细
        /// </summary>
        /// <param name="wmsno">拣货单号</param>
        /// <param name="lnkbllid">连接订单（为空/206分店拣货单、501外销单）</param>
        /// <returns></returns>
        [PWR(Pwrid = WMSConst.WMS_BACK_拣货查询, pwrdes = "拣货查询")]
        public ActionResult GetRetriveBllDtl(String wmsno, String lnkbllid)
        {
            if (lnkbllid == null)
            {
                lnkbllid = "206";
            }
            var qus = (from e in LoginInfo.DatPwrs
                       select e.qu).ToArray();
            //分店拣货单
            var qry = from e in WmsDc.wms_cang
                      where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                      && e.lnkbllid == lnkbllid
                       && (e.savdptid == LoginInfo.DefCsSavdptid)
                      && qus.Contains(e.qu.Trim())
                      && e.wmsno == wmsno
                      select new
                      {
                          e.bllid,
                          e.brief,
                          e.chkdat,
                          e.chkflg,
                          e.ckr,
                          e.lnkbllid,
                          e.lnkbocidat,
                          e.lnkbocino,
                          e.lnkbrief,
                          e.lnkno,
                          e.mkedat,
                          e.mkr,
                          e.opr,
                          e.prvid,
                          e.qu,
                          e.rcvdptid,
                          e.savdptid,
                          e.times,
                          e.wmsno,
                          prvdes = ""
                      };
            if (lnkbllid == "501")
            {
                //外销单
                qry = from e in WmsDc.wms_cang
                      join e1 in WmsDc.cus on e.prvid equals e1.cusid
                      where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                      && e.lnkbllid == lnkbllid
                       && (e.savdptid == LoginInfo.DefCsSavdptid)
                      && qus.Contains(e.qu.Trim())
                      && e.wmsno == wmsno
                      select new
                      {
                          e.bllid,
                          e.brief,
                          e.chkdat,
                          e.chkflg,
                          e.ckr,
                          e.lnkbllid,
                          e.lnkbocidat,
                          e.lnkbocino,
                          e.lnkbrief,
                          e.lnkno,
                          e.mkedat,
                          e.mkr,
                          e.opr,
                          e.prvid,
                          e.qu,
                          e.rcvdptid,
                          e.savdptid,
                          e.times,
                          e.wmsno,
                          prvdes = e1.cusdes
                      };
            }

            var arrqry = qry.ToArray();
            if (arrqry.Length <= 0)
            {
                return RNoData("未找到该拣货单，单号:" + wmsno);
            }
            var qrydtl = from e in WmsDc.wms_cangdtl
                         join e1 in WmsDc.gds on e.gdsid equals e1.gdsid
                         join e2 in
                             WmsDc.v_wms_pkg on new { e1.gdsid } equals new { e2.gdsid }
                         into joinPkg
                         from e3 in joinPkg.DefaultIfEmpty()
                         where e.wmsno == wmsno && e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                         select new
                         {
                             e.barcode,
                             e.bcd,
                             e.bkr,
                             e.bllid,
                             e.bokdat,
                             e.bokflg,
                             e.bthno,
                             e.gdsid,
                             e.gdstype,
                             e.oldbarcode,
                             e.pkgid,
                             e.pkgqty,
                             e.preqty,
                             qty = Math.Round(e.qty, 4, MidpointRounding.AwayFromZero),
                             e.rcdidx,
                             e.tpcode,
                             e.vlddat,
                             e.wmsno,
                             e1.gdsdes,
                             e1.spc,
                             e1.bsepkg,
                             pkg03 = GetPkgStr(Math.Round(e.qty, 4, MidpointRounding.AwayFromZero), e3.cnvrto, e3.pkgdes),
                             pkg03pre = GetPkgStr(Math.Round(e.preqty.Value, 4, MidpointRounding.AwayFromZero), e3.cnvrto, e3.pkgdes)
                         };
            //如果是拣货单，需要判断tpcode==GetY(),表示不能修改
            //if (lnkbllid == "206")
            //{
            qrydtl = qrydtl.Where(e => e.tpcode.ToLower() == "y");
            //}
            var arrqrydtl = qrydtl.ToArray();
            if (arrqrydtl.Length <= 0)
            {
                return RNoData("该拣货单没有明细信息");
            }

            return RSucc("成功", arrqrydtl);
        }

        /// <summary>
        /// 捡货单商品审核
        /// </summary>
        /// <param name="wmsno">收货单单号</param>
        /// <param name="barcode">仓位码</param>
        /// <param name="gdsid">商品编码</param>
        /// <param name="qty">单据商品序号</param>
        /// <param name="qty">实收数量</param>
        /// <returns>wms_blldtl, wms_blltp</returns>
        [PWR(Pwrid = WMSConst.WMS_BACK_拣货确认, pwrdes = "拣货确认")]
        public ActionResult BokRetrieveGds(String wmsno, String barcode, String gdsid, String gdstype, double qty)
        {
            using (TransactionScope scop = new TransactionScope())
            {
                //检索主表、明细表
                var qrymst = from e in WmsDc.wms_cang
                             where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                             && e.wmsno == wmsno
                             select e;                
                var arrmst = qrymst.ToArray();
                var qrydtl = from e in WmsDc.wms_cangdtl
                             where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                             && e.gdsid == gdsid
                             && e.gdstype == gdstype
                             && e.barcode == barcode
                             && e.wmsno == wmsno
                             && e.tpcode == "y"
                             select e;
                var arrdtl = qrydtl.ToArray();

                #region 检查输入参数
                if (arrmst.Length <= 0)
                {
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_NODATA;
                    Rm.ResultDesc = "未找到捡货单";
                    return ReturnResult();
                }
                if (arrdtl.Length <= 0)
                {
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_NODATA;
                    Rm.ResultDesc = "捡货单无明细信息";
                    return ReturnResult();
                }
                wms_cang mst = arrmst[0];
                //正在生成拣货单，请稍候重试
                string quRetrv = mst.qu;
                if (DoingRetrieve(LoginInfo.DefStoreid, quRetrv))
                {
                    return RInfo("正在生成拣货单，请稍候重试");
                }

                wms_cangdtl dtl = arrdtl[0];
                //是否捡货单已经审核
                if (mst.chkflg == GetY())
                {
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_INFO;
                    Rm.ResultDesc = "捡货单已经审核,不能再对该单据进行捡货处理！";
                    return ReturnResult();
                }
                #endregion

                #region 商品登帐
                if (dtl.bokflg == GetY() && dtl.bkr.Trim() != LoginInfo.Usrid)
                {
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_INFO;
                    Rm.ResultDesc = "该单据已经被" + dtl.bkr + "审核！";
                    return ReturnResult();
                }

                if (dtl.bokflg == GetN() && dtl.qty != null)
                {
                    //dtl.preqty = dtl.qty; 
                }
                if (dtl.tpcode == "n")
                {
                    return RInfo("拣货商品缺货");
                }
                if (dtl.preqty < qty && dtl.tpcode == "y")
                {
                    return RInfo("拣货数量大于应拣数量");
                }
                dtl.qty = Math.Round(qty, 4);
                dtl.pkgqty = Math.Round(qty, 4);

                //如果是206的单据，同一个商品的最后一条确认完后就不能再修改
                var qryallbygdsidN1 = from e in WmsDc.wms_cangdtl
                                      where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                                      && e.gdsid == gdsid
                                          //&& e.gdstype == gdstype                                    
                                      && e.wmsno == wmsno
                                      && e.bokflg == GetN() && e.tpcode == "y"
                                      select e;
                int iCnt = qryallbygdsidN1.Count();
                if (mst.lnkbllid.Trim() == "206" && iCnt == 0)
                {
                    return RInfo("该播种的该商品已经拣货完毕，不能修改");
                }

                dtl.bokflg = GetY();
                dtl.bokdat = DateTime.Now.ToString("yyyyMMddHHmmss");
                dtl.bkr = LoginInfo.Usrid;
                WmsDc.SubmitChanges();
                if (dtl.preqty != dtl.qty)
                {
                    i(wmsno, WMSConst.BLL_TYPE_RETRIEVE, "拣货商品明细确认", "应拣数量：" + dtl.preqty + "，实拣数量:" + dtl.qty, mst.qu, mst.savdptid);
                }
                #endregion

                #region 如果是206配送拣货的单据，在同一个拣货单里面同一商品确认完后，写入分货表
                if (mst.lnkbllid.Trim() == "206")
                {
                    var qryallbygdsidN = from e in WmsDc.wms_cangdtl
                                         where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                                         && e.gdsid == gdsid
                                             //&& e.gdstype == gdstype                                    
                                         && e.wmsno == wmsno
                                         && e.bokflg == GetN() && e.tpcode == "y"
                                         select e;
                    iCnt = qryallbygdsidN.Count();
                    if (iCnt == 0)
                    {
                        var qryAllByGdsidCang = from e in WmsDc.wms_cangdtl
                                                join e1 in WmsDc.wms_cang on new { e.wmsno, e.bllid } equals new { e1.wmsno, e1.bllid }
                                                where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                                                && e.tpcode == "y"
                                                && e.gdsid == gdsid
                                                && e.wmsno == wmsno
                                                group e by new
                                                {
                                                    e1.savdptid,
                                                    e1.rcvdptid,
                                                    e1.wmsno,
                                                    e1.bllid,
                                                    e1.lnkbocino,
                                                    e1.lnkbocidat,
                                                    e1.times,
                                                    e.gdsid
                                                } into g
                                                select new
                                                {
                                                    savdptid = g.Key.savdptid,
                                                    wmsno = g.Key.wmsno,
                                                    bllid = g.Key.bllid,
                                                    bocino = g.Key.lnkbocino,
                                                    bocidat = g.Key.lnkbocidat,
                                                    clsid = g.Key.times,
                                                    /*checi = (from e2 in WmsDc.psSndGds_dpt_dtl
                                                             where e2.dptid == g.Key.rcvdptid && e2.dh == g.Key.lnkbocino
                                                             select e2.busid.Substring(e2.busid.Trim().Length - 1, 1)).FirstOrDefault(),*/
                                                    gdsid = g.Key.gdsid,
                                                    qty = g.Sum(e => e.qty),
                                                    preqty = g.Sum(e => e.qty),
                                                    ckr = "",
                                                    chkflg = GetN(),
                                                    chkdat = ""
                                                };
                        var cutgds = qryAllByGdsidCang.FirstOrDefault();

                        #region 如果拣货的数量不够的话,要去修改配送单的数量和金额                        
                        var qrystkdtl = from e in WmsDc.stkotdtl
                                        where e.stkot.wmsbllid == cutgds.bllid
                                        && e.stkot.wmsno == cutgds.wmsno
                                        && e.gdsid == cutgds.gdsid
                                        orderby e.qty descending
                                        select e;
                        double q = qrystkdtl.Sum(e => e.qty) - cutgds.qty;

                        if (q > 0)
                        {
                            double diff = q;
                            var stkotdtl = qrystkdtl;
                            //减小数部分
                            #region 减小数部分
                            foreach (stkotdtl d in stkotdtl)
                            {
                                if (d.preqty == null)
                                {
                                    d.preqty = d.qty;
                                }
                                double xtmp = d.qty * 10000 % 10000 / 10000;
                                if (diff > 0 && diff >= xtmp)
                                {
                                    diff -= xtmp;
                                    d.qty -= xtmp;

                                    d.qty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.pkgqty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.taxamt = Math.Round(d.qty * d.prc * d.taxrto, 4);
                                    d.amt = Math.Round(d.qty * d.prc, 4);
                                    d.salamt = d.qty * d.salprc;
                                    d.patamt = Math.Round(d.qty * d.taxprc, 4);
                                    d.stotcstamt = Math.Round(d.qty * d.stotcstprc.Value, 4);
                                }
                                else if (diff > 0 && diff < xtmp)
                                {
                                    d.qty -= diff;
                                    diff = 0;

                                    d.qty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.pkgqty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.taxamt = Math.Round(d.qty * d.prc * d.taxrto, 4);
                                    d.amt = Math.Round(d.qty * d.prc, 4);
                                    d.salamt = d.qty * d.salprc;
                                    d.patamt = Math.Round(d.qty * d.taxprc, 4);
                                    d.stotcstamt = Math.Round(d.qty * d.stotcstprc.Value, 4);
                                }
                            }
                            //WmsDc.SubmitChanges();
                            #endregion 减小数部分
                            //减去零散件规
                            #region 减去零散件规
                            foreach (stkotdtl d in stkotdtl)
                            {
                                if (d.preqty == null)
                                {
                                    d.preqty = d.qty;
                                }
                                double xtmp = (double)WmsDc.ExecuteQuery<decimal>("select convert(decimal,{0}) % convert(decimal,e.cnvrto) from v_wms_pkg e where e.gdsid={1}",
                                         d.qty, d.gdsid).FirstOrDefault();
                                /*double xtmp = (from e in WmsDc.v_wms_pkg
                                               where e.gdsid == d.gdsid
                                               select Convert.ToInt32(d.qty) % e.cnvrto).FirstOrDefault();*/
                                if (diff > 0 && diff >= xtmp)
                                {
                                    diff -= xtmp;
                                    d.qty -= xtmp;

                                    d.qty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.pkgqty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.taxamt = Math.Round(d.qty * d.prc * d.taxrto, 4);
                                    d.amt = Math.Round(d.qty * d.prc, 4);
                                    d.salamt = d.qty * d.salprc;
                                    d.patamt = Math.Round(d.qty * d.taxprc, 4);
                                    d.stotcstamt = Math.Round(d.qty * d.stotcstprc.Value, 4);
                                }
                                else if (diff > 0 && diff < xtmp)
                                {
                                    d.qty -= diff;
                                    diff = 0;

                                    d.qty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.pkgqty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.taxamt = Math.Round(d.qty * d.prc * d.taxrto, 4);
                                    d.amt = Math.Round(d.qty * d.prc, 4);
                                    d.salamt = d.qty * d.salprc;
                                    d.patamt = Math.Round(d.qty * d.taxprc, 4);
                                    d.stotcstamt = Math.Round(d.qty * d.stotcstprc.Value, 4);
                                }
                            }
                            //WmsDc.SubmitChanges();
                            #endregion 减去零散件规
                            //减去从大到小的数量
                            #region 减去从大到小的数量
                            foreach (stkotdtl d in stkotdtl)
                            {
                                if (d.preqty == null)
                                {
                                    d.preqty = d.qty;
                                }
                                if (diff > 0 && diff >= d.qty)
                                {
                                    diff -= d.qty;
                                    d.qty = 0;

                                    d.qty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.pkgqty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.taxamt = Math.Round(d.qty * d.prc * d.taxrto, 4);
                                    d.amt = Math.Round(d.qty * d.prc, 4);
                                    d.salamt = d.qty * d.salprc;
                                    d.patamt = Math.Round(d.qty * d.taxprc, 4);
                                    d.stotcstamt = Math.Round(d.qty * d.stotcstprc.Value, 4);
                                }
                                else if (diff > 0 && diff < d.qty)
                                {
                                    d.qty = d.qty - diff;
                                    diff = 0;

                                    d.qty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.pkgqty = Math.Round(d.qty, 4, MidpointRounding.AwayFromZero);
                                    d.taxamt = Math.Round(d.qty * d.prc * d.taxrto, 4);
                                    d.amt = Math.Round(d.qty * d.prc, 4);
                                    d.salamt = d.qty * d.salprc;
                                    d.patamt = Math.Round(d.qty * d.taxprc, 4);
                                    d.stotcstamt = Math.Round(d.qty * d.stotcstprc.Value, 4);
                                }
                            }
                            WmsDc.SubmitChanges();
                            #endregion 减去从大到小的数量
                        }
                        WmsDc.SubmitChanges();
                        #endregion

                        // 写入分货表
                        #region 写入分货表
                        //var qryAllByGdsid = from e in WmsDc.stkotdtl
                        //                    join e1 in WmsDc.wms_cang on new { e.stkot.wmsno, e.stkot.wmsbllid } equals new { e1.wmsno, wmsbllid = e1.bllid }
                        //                    where e.stkot.wmsbllid == cutgds.bllid
                        //                    && e.stkot.wmsno == cutgds.wmsno
                        //                    && e.gdsid == cutgds.gdsid
                        //                    && e.qty != 0
                        //                    group e by new
                        //                    {
                        //                        e1.savdptid,
                        //                        e.stkot.rcvdptid,
                        //                        e1.wmsno,
                        //                        e1.bllid,
                        //                        e1.lnkbocino,
                        //                        e1.lnkbocidat,
                        //                        e1.times,
                        //                        e.gdsid
                        //                    } into g
                        //                    select new
                        //                    {
                        //                        savdptid = g.Key.savdptid,
                        //                        wmsno = g.Key.wmsno,
                        //                        bllid = g.Key.bllid,
                        //                        bocino = g.Key.lnkbocino,
                        //                        bocidat = g.Key.lnkbocidat,
                        //                        clsid = g.Key.times,
                        //                        checi = (from e2 in WmsDc.psSndGds_dpt_dtl
                        //                                 where e2.dptid == g.Key.rcvdptid && e2.dh == g.Key.lnkbocino
                        //                                 select e2.busid.Substring(e2.busid.Trim().Length - 1, 1)).FirstOrDefault(),
                        //                        gdsid = g.Key.gdsid,
                        //                        qty = g.Sum(e => e.qty),
                        //                        preqty = g.Sum(e => e.qty),
                        //                        ckr = "",
                        //                        chkflg = GetN(),
                        //                        chkdat = ""
                        //                    };
                        ////i(wmsno, "", "拣货确认", qryAllByGdsid.ToString(), "", LoginInfo.DefSavdptid);                        
                        //var arrQryAllByGdsid = qryAllByGdsid.ToArray();
                        //foreach (var a in arrQryAllByGdsid)
                        //{
                        //    if (a.checi == null)
                        //    {
                        //        iFile(cutgds.bllid + "    " + cutgds.wmsno + "  " + cutgds.gdsid + "  ");
                        //    }
                        //}

                        //var qryAllByGdsidSum = from e in arrQryAllByGdsid
                        //                       group e by new
                        //                       {
                        //                           e.savdptid,
                        //                           e.wmsno,
                        //                           e.bllid,
                        //                           e.bocino,
                        //                           e.bocidat,
                        //                           e.clsid,
                        //                           e.checi,
                        //                           e.gdsid,
                        //                           e.ckr,
                        //                           e.chkflg,
                        //                           e.chkdat
                        //                       } into g
                        //                       select new
                        //                       {
                        //                           g.Key.savdptid,
                        //                           g.Key.wmsno,
                        //                           g.Key.bllid,
                        //                           g.Key.bocino,
                        //                           g.Key.bocidat,
                        //                           g.Key.clsid,
                        //                           g.Key.checi,
                        //                           g.Key.gdsid,
                        //                           g.Key.ckr,
                        //                           g.Key.chkflg,
                        //                           g.Key.chkdat,
                        //                           qty = g.Sum(e => e.qty),
                        //                           preqty = g.Sum(e => e.preqty)
                        //                       };
                        //List<wms_cutgds> lstCg = new List<wms_cutgds>();
                        //foreach (var tcg in qryAllByGdsidSum)
                        //{
                        //    wms_cutgds cg = new wms_cutgds();
                        //    cg.bllid = tcg.bllid;
                        //    cg.bocidat = tcg.bocidat;
                        //    cg.bocino = tcg.bocino;
                        //    cg.checi = tcg.checi;
                        //    cg.chkdat = tcg.chkdat;
                        //    cg.chkflg = tcg.chkflg;
                        //    cg.ckr = tcg.ckr;
                        //    cg.clsid = tcg.clsid;
                        //    cg.gdsid = tcg.gdsid;
                        //    cg.preqty = tcg.preqty;
                        //    cg.qty = tcg.qty;
                        //    cg.savdptid = tcg.savdptid;
                        //    cg.wmsno = tcg.wmsno;
                        //    lstCg.Add(cg);
                        //}
                        //WmsDc.wms_cutgds.InsertAllOnSubmit(lstCg);
                        #endregion 写入分货表
                    }
                }
                #endregion

                try
                {
                    WmsDc.SubmitChanges();
                    scop.Complete();
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_SUCCESS;
                    return ReturnResult();
                }
                catch (Exception ex)
                {
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_ERRORS;
                    Rm.ResultDesc = "错误:" + ex.Message;
                    return ReturnResult();
                }
            }
        }

        /// <summary>
        /// 捡货单审核
        /// </summary>
        /// <param name="wmsno">捡货单号</param>
        /// <returns></returns>
        [PWR(Pwrid = WMSConst.WMS_BACK_拣货审核, pwrdes = "拣货审核")]
        public ActionResult BokRetrieve(String wmsno)
        {
            using (TransactionScope scop = new TransactionScope())
            {
                Rm.ResultObject = null;
                //检索捡货单主表、明细表
                var qrymst = from e in WmsDc.wms_cang
                             where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                             && e.wmsno == wmsno
                             select e;
                var arrmst = qrymst.ToArray();
                var qrydtl = from e in WmsDc.wms_cangdtl
                             where e.bllid == WMSConst.BLL_TYPE_RETRIEVE
                             && e.wmsno == wmsno
                             select e;
                var arrdtl = qrydtl.ToArray();

                #region 检查输入参数
                if (arrmst.Length <= 0)
                {
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_NODATA;
                    Rm.ResultDesc = "未找到捡货单";
                    return ReturnResult();
                }
                if (arrdtl.Length <= 0)
                {
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_NODATA;
                    Rm.ResultDesc = "捡货单无明细信息";
                    return ReturnResult();
                }
                wms_cang mst = arrmst[0];
                //正在生成拣货单，请稍候重试
                string quRetrv = mst.qu;
                if (DoingRetrieve(LoginInfo.DefStoreid, quRetrv))
                {
                    return RInfo("正在生成拣货单，请稍候重试");
                }

                foreach (wms_cangdtl d in arrdtl)
                {
                    //是否捡货单商品已经审核
                    if (d.bokflg == GetN() && d.tpcode == "y")
                    {
                        Rm.ResultCode = ResultMessage.RESULTMESSAGE_INFO;
                        Rm.ResultDesc = "捡货单商品尚未审核完,商品货号:" + d.gdsid + "！";
                        return ReturnResult();
                    }
                }
                if (mst.chkflg == GetY())
                {
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_INFO;
                    Rm.ResultDesc = "捡货单,已经审核,请不要重复审核";
                    return ReturnResult();
                }
                #endregion

                #region 播种单查询
                ////播种单主表，明细表
                var qrybzmst = from e in WmsDc.wms_bzmst
                               where e.bllid == WMSConst.BLL_TYPE_BZ
                               && e.lnkbocidat == mst.lnkbocidat
                               && e.lnkbocino == mst.lnkbocino
                               && e.qu == mst.qu
                               && e.savdptid == mst.savdptid
                               select e;
                var arrbzmst = qrybzmst.ToArray();
                if (arrbzmst.Length <= 0)
                {
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_NODATA;
                    Rm.ResultDesc = "播种单为空";
                    return ReturnResult();
                }
                wms_bzmst bzmst = arrbzmst[0];
                //播种单明细表
                var qrybzdtl = from e in WmsDc.wms_bzdtl
                               where e.wmsno == bzmst.wmsno
                               && arrdtl.Select(e1 => e1.gdsid).Contains(e.gdsid.Trim())
                               select e;
                wms_bzdtl[] arrbzdtls = qrybzdtl.ToArray();
                #endregion

                #region 差异数量生成捡货损溢单，记日志

                //差异数量生成捡货损溢单，记日志
                #region 差异数量生成捡货损溢单，记日志
                //生成捡货损溢单
                JsonResult jr = (JsonResult)MakeNewBllNo(this.LoginInfo.DefSavdptid, WMSConst.BLL_TYPE_RETRIEVE_PROFIT_LOSS, (bllno) =>
                {
                    //生成损溢单主表
                    wms_cang sycang = new wms_cang();
                    sycang.wmsno = bllno;
                    sycang.bllid = WMSConst.BLL_TYPE_RETRIEVE_PROFIT_LOSS;
                    sycang.savdptid = mst.savdptid;
                    sycang.prvid = mst.prvid;
                    sycang.qu = mst.qu;
                    sycang.rcvdptid = mst.rcvdptid;
                    sycang.times = mst.times;
                    sycang.lnkbocino = mst.lnkbocino;
                    sycang.lnkbocidat = mst.lnkbocidat;
                    sycang.mkr = LoginInfo.Usrid;
                    sycang.mkedat = DateTime.Now.ToString("yyyyMMdd");
                    sycang.mkedat2 = GetCurrentDate();
                    sycang.ckr = LoginInfo.Usrid;
                    sycang.chkflg = GetY();
                    sycang.chkdat = DateTime.Now.ToString("yyyyMMddHHmmss");
                    sycang.opr = LoginInfo.Usrid;
                    sycang.brief = "";
                    sycang.lnkbllid = mst.bllid;
                    sycang.lnkno = mst.wmsno;
                    sycang.lnkbrief = mst.brief;

                    //生成损溢单明细
                    List<wms_cangdtl> lstsydtl = new List<wms_cangdtl>();
                    foreach (wms_cangdtl dt in arrdtl)
                    {
                        if (dt.qty != dt.preqty)
                        {
                            wms_cangdtl sydtl = new wms_cangdtl();
                            sydtl.wmsno = bllno;
                            sydtl.bllid = WMSConst.BLL_TYPE_RETRIEVE_PROFIT_LOSS;
                            sydtl.rcdidx = dt.rcdidx;
                            sydtl.oldbarcode = dt.oldbarcode;
                            sydtl.barcode = dt.barcode;
                            sydtl.gdsid = dt.gdsid;
                            sydtl.pkgid = dt.pkgid;
                            sydtl.pkgqty = dt.pkgqty;
                            sydtl.qty = dt.preqty.Value - dt.qty;
                            sydtl.gdstype = dt.gdstype;
                            sydtl.bthno = dt.bthno;
                            sydtl.vlddat = dt.vlddat;
                            sydtl.bcd = dt.bcd;
                            sydtl.tpcode = dt.tpcode;
                            sydtl.bkr = LoginInfo.Usrid;
                            sydtl.bokflg = GetY();
                            sydtl.bokdat = DateTime.Now.ToString("yyyyMMddHHmmss");
                            sydtl.preqty = dt.preqty.Value - dt.qty;
                            lstsydtl.Add(sydtl);
                            //记录日志
                            Log.i(LoginInfo.Usrid, Mdlid, bllno, WMSConst.BLL_TYPE_RETRIEVE_PROFIT_LOSS, "损溢单生成", dt.gdsid.Trim() + ":应收:" + dt.preqty.Value + ";实收:" + dt.qty, mst.qu, mst.savdptid);
                            //记账并删除为0的仓位信息。
                            var cwggdsbs = WmsDc.wms_cwgdsbs
                                .Where(e => e.barcode == sydtl.barcode && e.bcd == sydtl.bcd && e.savdptid == mst.savdptid && e.gdsid == sydtl.gdsid && e.gdstype == sydtl.gdstype)
                                .Select(e => e).Single();
                            if (cwggdsbs != null)
                            {
                                cwggdsbs.qty -= sydtl.qty;
                                if (cwggdsbs.qty <= 0)
                                {
                                    WmsDc.wms_cwgdsbs.DeleteOnSubmit(cwggdsbs);
                                    iDelCwgdsbs(new wms_cwgdsbs[] { cwggdsbs });
                                }
                            }
                        }
                    }
                    WmsDc.wms_cang.InsertOnSubmit(sycang);
                    WmsDc.wms_cangdtl.InsertAllOnSubmit(lstsydtl);


                    ResultMessage rm = new ResultMessage();
                    rm.ResultCode = ResultMessage.RESULTMESSAGE_SUCCESS;
                    rm.ResultDesc = "损溢单生成成功";
                    return rm;
                });


                Rm = (ResultMessage)jr.Data;
                if (Rm.ResultCode != ResultMessage.RESULTMESSAGE_SUCCESS)
                {
                    return jr;
                }
                #endregion


                //拣货数量和应拣数量判断是不是相等， 不相等就改播种单数量
                #region 拣货数量和应拣数量判断是不是相等， 不相等就改播种单数量
                var qsum = arrdtl
                    .GroupBy(g => new { g.gdsid, g.gdstype })
                    .Select(g => new { gdsid = g.Key.gdsid, gdstype = g.Key.gdstype, sumqty = g.Sum(e => e.qty) });
                foreach (var q in qsum)
                {
                    //查找播种明细商品汇总数量是否和捡货单的商品汇总数量一致
                    var qbzsum = arrbzdtls
                        .Where(e => e.gdsid == q.gdsid && e.gdstype == q.gdstype)
                        .GroupBy(g => new { g.gdsid, g.gdstype })
                        .Select(g => new { gdsid = g.Key.gdsid, gdstype = g.Key.gdstype, qsum = g.Sum(e => e.qty) }).Single();
                    if (qbzsum.qsum < q.sumqty)    //如果数量不一致就生成捡货损溢单，并记日志
                    {
                        Rm.ResultCode = ResultMessage.RESULTMESSAGE_INFO;
                        Rm.ResultDesc = "捡货单数量大于播种的数量";
                        return ReturnResult();
                    }
                    else if (qbzsum.qsum >= q.sumqty)    //如果播种的数量小于捡货单的数量
                    {
                        double remainqty = q.sumqty;
                        var arrbzdtlsGdstyp = arrbzdtls.Where(e => e.gdstype == q.gdstype && e.gdsid == q.gdsid);
                        //-----------分派数量----------
                        //按照播种数量从大到小的顺序   |
                        //从小到大开始播种             |
                        //播种到最后就播0              |
                        //-----------------------------
                        /*String cmdsql = "declare @remainqty deciaml(18,4)\r\n"
                        + "declare @wmsno varchar(20), @gdstype varchar(20), @rcdidx int, @gdsid varchar(10), @rcvdptid varchar(10), @qty decimal\r\n"
                        + "set @remainqty={0}\r\n"
                        + "set @gdsid={1}\r\n"
                        + "set @gdstype={2}\r\n"
                        + "set @wmsno={3}\r\n"
                        + "declare cur1 cursor for\r\n"
                        + "    select rcvdptid, qty, rcdidx from wms_bzdtl where wmsno=@wmsno and gdstype=@gdstype and gdsid=@gdsid order by qty desc, rcdidx \r\n"
                        + "open cur1\r\n"
                        + "fetch next from cur1 into @rcvdptid, @qty, @rcdidx\r\n"
                        + "while @@fetch_status=0\r\n"
                        + "begin\r\n"
                        + "    if @remainqty-@qty>0\r\n"
                        + "    begin\r\n"
                        + "        update wms_bzdtl set qty = @qty where wmsno=@wmsno and gdstype=@gdstype and gdsid=@gdsid and rcdidx=@rcdidx\r\n"
                        + "    end\r\n"
                        + "    else if @remainqty>0 and @remainqty-@qty<=0\r\n"
                        + "    begin\r\n"
                        + "        update wms_bzdtl set qty = @remainqty where wmsno=@wmsno and gdstype=@gdstype and gdsid=@gdsid and rcdidx=@rcdidx\r\n"
                        + "    end\r\n"
                        + "    else\r\n"
                        + "    begin\r\n"
                        + "        update wms_bzdtl set qty = 0 where wmsno=@wmsno and gdstype=@gdstype and gdsid=@gdsid and rcdidx=@rcdidx\r\n"
                        + "    end\r\n"
                        + "    set @remainqty = @remainqty-@qty\r\n"
                        + "    fetch next from cur1 into @rcvdptid, @qty, @rcdidx\r\n"
                        + "end\r\n"
                        + "close cur1\r\n"
                        + "deallocate cur1";*/
                        /*
                         * preqty = preqty==null ? qty : preqty
                         * 
                         * 公式：taxamt = qty*prc*taxrto
                         * amt = qty*prc
                         * salamt = qty*salprc
                         * patamt = qty*taxprc
                         * stotcstamt = qty*stotcstprc
                         * 
                         */

                        String cmdsql = "";
                        if (mst.lnkbllid.Trim() == "206" || mst.lnkbllid.Trim() == "501")
                        {
                            cmdsql = "declare @remainqty deciaml(18,4)\r\n"
                             + "declare @wmsno varchar(20), @gdstype varchar(20), @rcdidx int, @gdsid varchar(10), @rcvdptid varchar(10), @qty decimal, @stkouno varchar(30)\r\n"
                             + "set @remainqty={0}\r\n"
                             + "set @gdsid={1}\r\n"
                             + "set @wmsno={2}\r\n"
                             + "declare cur1 cursor for\r\n"
                             + "    select b.rcvdptid, a.qty, a.rcdidx, b.stkouno from stkotdtl a inner join stkot b on a.stkouno=b.stkouno\r\n"
                             + "          where b.wmsno=@wmsno and b.wmsbllid='" + WMSConst.BLL_TYPE_RETRIEVE + "' and a.gdsid=@gdsid order by qty desc, rcdidx \r\n"
                             + "open cur1\r\n"
                             + "fetch next from cur1 into @rcvdptid, @qty, @rcdidx, @stkouno\r\n"
                             + "while @@fetch_status=0\r\n"
                             + "begin\r\n"
                             + "     update stkotdtl set preqty=case when preqty is null then qty else preqty end where stkouno=@stkouno and wmsbllid='" + WMSConst.BLL_TYPE_RETRIEVE + "' and gdsid=@gdsid and rcvidx=@rcvidx  \r\n"
                             + "    if @remainqty-@qty>0\r\n"
                             + "    begin\r\n"
                             + "        update stkotdtl set qty = @qty where stkouno=@stkouno and gdsid=@gdsid and rcdidx=@rcdidx\r\n"
                             + "    end\r\n"
                             + "    else if @remainqty>0 and @remainqty-@qty<=0\r\n"
                             + "    begin\r\n"
                             + "        update stkotdtl set qty = @remainqty where stkouno=@stkouno and gdsid=@gdsid and rcdidx=@rcdidx\r\n"
                             + "    end\r\n"
                             + "    else\r\n"
                             + "    begin\r\n"
                             + "        update stkotdtl set qty = 0 where stkouno=@stkouno and gdsid=@gdsid and rcdidx=@rcdidx\r\n"
                             + "    end\r\n"
                             + "    update stkotdtl set amt=qty*prc, salamt=qty*salprc, patamt=qty*taxprc, stotcstamt=qty*stotcstprc where stkouno=@stkouno and gdsid=@gdsid and rcdidx=@rcdidx \r\n"
                             + "    set @remainqty = @remainqty-@qty\r\n"
                             + "    fetch next from cur1 into @rcvdptid, @qty, @rcdidx, @stkouno\r\n"
                             + "end\r\n"
                             + "close cur1\r\n"
                             + "deallocate cur1";
                            //WmsDc.ExecuteCommand(cmdsql, new object[] { remainqty, q.gdsid, bzmst.wmsno });
                        }

                    }
                }
                #endregion

                #endregion

                #region 修改审核标记
                //修改审核标记
                mst.chkflg = GetY();
                mst.chkdat = DateTime.Now.ToString("yyyyMMddHHmmss");
                mst.ckr = LoginInfo.Usrid;
                #endregion

                try
                {
                    WmsDc.SubmitChanges();
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_SUCCESS;
                    return ReturnResult();
                }
                catch (Exception ex)
                {
                    Rm.ResultCode = ResultMessage.RESULTMESSAGE_ERRORS;
                    Rm.ResultDesc = ex.Message;
                    Rm.ResultObject = null;
                    return ReturnResult();
                }

            }
        }

        /// <summary>
        /// 拣货查询
        /// </summary>
        /// <param name="begindat">查询开始时间</param>
        /// <param name="enddat">查询结束时间</param>
        /// <param name="bllid">单据类型</param>
        /// <param name="barcode">仓位编码</param>
        /// <param name="bkr">拣货确认人</param>
        /// <param name="gdsid">商品编码</param>
        /// <returns></returns>
        [PWR(Pwrid = WMSConst.WMS_BACK_拣货查询, pwrdes = "拣货查询")]
        public ActionResult FindBll(String begindat, String enddat, String barcode, String bllid, String bkr, String gdsid)
        {
            //判断分区是否有效
            if (!String.IsNullOrEmpty(barcode) && !IsExistBarcode(barcode))
            {
                return RInfo("仓位码" + barcode.Trim() + "无效");
            }

            var arrqrymst = FindBllFromCangMst103(begindat, enddat, barcode, bllid, bkr, gdsid);
            if (arrqrymst.Length <= 0)
            {
                return RNoData("未找到符合条件的单据");
            }
            return RSucc("成功", arrqrymst);
        }

        protected override void SetModuleInfo()
        {
            Mdlid = "CsRetrieve";
            Mdldes = "残损捡货单模块";
        }

    }
}
