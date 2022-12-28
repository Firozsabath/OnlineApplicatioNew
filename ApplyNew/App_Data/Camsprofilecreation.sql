Alter procedure Camsprofilecreation
As 
begin
Declare @Addressid int
Declare @prospectid int

Insert into PreAdmission(AnticipatedEntryTermID,FirstName,LastName,Salutation,MiddleInitial,PreAdmitDate,StatusID,IsAdmitted,SourceOfLead,
EntryTime,ProspectCanceled,InsertUserID,InsertTime,EverApplied,Veteran)
values('','','','','',GETDATE(),10,'No','',getdate(),'No','AppPortal',getdate(),'No','No')

select top 1 @prospectid =  ProspectID from PreAdmission order by PreAdmitDate desc

Insert into PreApplication_CUDCustom(ProspectID,ProgramID,EventIDs) values(@prospectid,'','')

select * from StudentPortal order by ProspectID desc

Insert into Address(AddressTypeID,Phone1,Phone2,Email1,Email2,ActiveFlag,InsertUserID,InsertTime,UpdateUserID,InternationalFlag)
values(287,'','','','','Yes','AppPortal',getdate(),'ApplyPortal','No')

select top 1 @Addressid =  AddressID  from Prospect_Address order by AddressID desc

Insert into Prospect_Address(ProspectID,AddressID)values(@prospectid,@Addressid)

Insert into StudentPortal(PortalHandle,PortalPIN,PINValidated,PortalEnable,OriginalPIN,
PINValidateDT,ProspectID,PortalUserTypeID,EncPass,LastPasswordChange,GradAppFirstTimeLogin)
values('',@prospectid,'Yes','Yes',@prospectid,getdate(),@prospectid,1,'',getdate(),1)
End