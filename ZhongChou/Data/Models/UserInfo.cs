using System;
using System.Linq;
using System.Collections.Generic;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [ORTableMapping("zc.UserInfo")]
    public class UserInfo
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [ORFieldMapping("Nickname")]
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [ORFieldMapping("HeadImage")]
        public string HeadImage { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [ORFieldMapping("Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [ORFieldMapping("Email")]
        public string Email { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [ORFieldMapping("Gender")]
        public string Gender { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [ORFieldMapping("Province")]
        public string Province { get; set; }

        /// <summary>
        /// 市区
        /// </summary>
        [ORFieldMapping("City")]
        public string City { get; set; }

        /// <summary>
        /// 个人签名
        /// </summary>
        [ORFieldMapping("Signature")]
        public string Signature { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [ORFieldMapping("RealName")]
        public string RealName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [ORFieldMapping("IDNumber")]
        public string IDNumber { get; set; }

        /// <summary>
        /// 0-普通用户，1-企业用户，2-平台用户
        /// </summary>
        [ORFieldMapping("Enums.UserType")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public Enums.UserType UserType { get; set; }

        /// <summary>
        /// 0-草稿箱，1-审核中，2-审核未通过，3-审核通过
        /// </summary>
        [ORFieldMapping("AuditStatus")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public Enums.AuditStatus AuditStatus { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("IsValid")]
        public bool IsValid { get; set; }

        /// <summary>
        /// 粉丝个数
        /// </summary>
        [ORFieldMapping("FollowerNo")]
        public int FollowerNo { get; set; }

        /// <summary>
        /// 关注用户个数
        /// </summary>
        [ORFieldMapping("FocusNo")]
        public int FocusNo { get; set; }

        /// <summary>
        /// 收藏项目个数
        /// </summary>
        [ORFieldMapping("CollectNo")]
        public int CollectNo { get; set; }

        /// <summary>
        /// 发起项目个数
        /// </summary>
        [ORFieldMapping("LaunchNo")]
        public int LaunchNo { get; set; }

        /// <summary>
        /// 支持项目个数
        /// </summary>
        [ORFieldMapping("SupportNo")]
        public int SupportNo { get; set; }
        /// <summary>
        /// 积分合计
        /// </summary>
        [ORFieldMapping("PointTotal")]
        public int PointTotal { get; set; }

        /// <summary>
        /// 新消息
        /// </summary>
        [ORFieldMapping("NewMessage")]
        public int NewMessage { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        [ORFieldMapping("PraiseNo")]
        public int PraiseNo { get; set; }

        /// <summary>
        /// 好友申请数
        /// </summary>
        [ORFieldMapping("ApplayNo")]
        public int ApplayNo { get; set; }
        /// <summary>
        /// 最近查看动态时间
        /// </summary>
        [ORFieldMapping("DynamicTime")]
        public DateTime DynamicTime { get; set; }

        [NoMapping]
        public string DynamicTimeFormat { get { return DynamicTime.ToString("M/d"); } }

        /// <summary>
        /// 最近查看任务时间
        /// </summary>
        [ORFieldMapping("TaskTime")]
        public DateTime TaskTime { get; set; }

        ///// <summary>
        ///// 是否实名认证
        ///// </summary>
        //[ORFieldMapping("IsRealNameAuth")]
        //public Boolean IsRealNameAuth { get; set; }
        ///// <summary>
        ///// 是否填写生活家资料
        ///// </summary>
        //[ORFieldMapping("IsFillLiferInfo")]
        //public Boolean IsFillLiferInfo { get; set; }


        /// <summary>
        /// 是否是生活家
        /// </summary>
        [ORFieldMapping("IsLifer")]
        public Boolean IsLifer { get; set; }

        /// <summary>
        /// 银行卡数量
        /// </summary>
        [ORFieldMapping("BankCardNo")]
        public int BankCardNo { get; set; }
        /// <summary>
        /// IMID
        /// </summary>
        [ORFieldMapping("ID")]
        public int ID { get; set; }

    }

    /// <summary>
    /// 用户信息集合
    /// </summary>
    public class UserInfoCollection : EditableDataObjectCollectionBase<UserInfo>
    {
    }

    /// <summary>
    /// 用户信息操作类
    /// </summary>
    public class UserInfoAdapter : UpdatableAndLoadableAdapterBase<UserInfo, UserInfoCollection>
    {
        public static readonly UserInfoAdapter Instance = new UserInfoAdapter();

        private UserInfoAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public UserInfo LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }
        public UserInfo LoadByName(string name)
        {
            return this.Load(where =>
            {
                where.AppendItem("Nickname", name);
            }).FirstOrDefault();
        }
        public void DeleteByCode(string code, bool trueDelete = false)
        {
            //逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Code", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
            }
        }
        public UserInfoCollection LoadAllUser()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }
        public UserInfoCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public bool ExistsByCode(string code)
        {
            return this.Exists(where =>
            {
                where.AppendItem("Code", code);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>
        public UserInfoCollection LoadAllOfUserType(Enums.UserType userType)
        {
            return this.Load(where => where.AppendItem("UserType", userType.ToString("D")));
        }

        public UserInfoCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }

        public UserInfoCollection LoadFriends(List<string> codes)
        {
            return this.LoadByInBuilder(w =>
            {
                w.DataField = "Code";
                w.AppendItem(codes.ToArray());
            }); ;
        }
        public void UpdateUserInfo(string userCode, Enums.UserType userType, string realName, string nickName, string idNumber, string gender, string city)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("Enums.UserType", userType.ToString("D"));
                    update.AppendItem("RealName", realName);
                    update.AppendItem("NickName", nickName);
                    update.AppendItem("IDNumber", idNumber);
                    update.AppendItem("Gender", gender);
                    update.AppendItem("City", city);
                },
                where =>
                {
                    where.AppendItem("Creator", userCode);
                },
                this.GetConnectionName());
        }

        public void UpdateHeadImage(string userCode, string headImage)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("HeadImage", headImage);
                },
                where =>
                {
                    where.AppendItem("Code", userCode);
                },
                this.GetConnectionName());
        }
        public void UpdateRealName(string userCode, string realName)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("RealName", realName);
                },
                where =>
                {
                    where.AppendItem("Code", userCode);
                },
                this.GetConnectionName());
        }

        /// <summary>
        /// 更新个人资料
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="nickName"></param>
        /// <param name="phone"></param>
        /// <param name="headImage"></param>
        public void UpdatePersonalData(string userCode, string nickName, string phone, string headImage, Enums.UserType userType)
        {
            var user = this.LoadByCode(userCode);
            if (user != null)
            {
                this.UpdateUserInfo(userCode, nickName, phone, headImage, userType);
            }
            else
            {
                UserInfo us = new UserInfo()
                {
                    Code = userCode,
                    NickName = nickName,
                    Phone = phone,
                    HeadImage = headImage,
                    UserType = userType,
                    CreateTime = DateTime.Now,
                    IsValid = true
                };
                this.Update(us);
            }
        }

        public void UpdateUserInfo(string userCode, string nickName, string phone, string headImage, Enums.UserType userType)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("NickName", nickName);
                    update.AppendItem("Phone", phone);
                    if (headImage != "")
                    {
                        update.AppendItem("HeadImage", headImage);
                    }
                },
                where =>
                {
                    where.AppendItem("Code", userCode);
                },
                this.GetConnectionName());
        }

        /// <summary>
        /// 作废的
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="isRealNameAuth"></param>
        public void UpdateIsRealNameAuth(string userCode, bool isRealNameAuth)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("IsRealNameAuth", isRealNameAuth);
                },
                where =>
                {
                    where.AppendItem("Code", userCode);
                },
                this.GetConnectionName());
        }

        public void UpdateIsLifer(string userCode, bool isLifer)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("IsLifer", isLifer);
                },
                where =>
                {
                    where.AppendItem("Code", userCode);
                },
                this.GetConnectionName());
        }

        public void UpdateBankCardNo(string userCode, int bankCardNo)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("BankCardNo", bankCardNo);
                },
                where =>
                {
                    where.AppendItem("Code", userCode);
                },
                this.GetConnectionName());
        }

        /// <summary>
        /// 好友申请个数+1
        /// </summary>
        /// <param name="userCode"></param>
        public void SetIncApplyNo(string userCode)
        {
            this.SetInc("ApplayNo", 1, where =>
            {
                where.AppendItem("Code", userCode);
            }, this.GetConnectionName());
        }

        /// <summary>
        /// 好友申请个数-1
        /// </summary>
        /// <param name="userCode"></param>
        public void SetDecApplyNo(string userCode)
        {
            this.SetDec("ApplayNo", 1, where =>
            {
                where.AppendItem("Code", userCode);
            }, this.GetConnectionName());
        }
    }


}

