// Auto-generated code from PostgreSQL schema
using System.ComponentModel;

public enum SchemaName
{
	[Description("public")]
	Public = 0,
	[Description("cts")]
	Cts = 1,
	[Description("cts_audit")]
	CtsAudit = 2,
	[Description("cts_transactions")]
	CtsTransactions = 3,
	[Description("cads")]
	Cads = 4
}

[AttributeUsage(AttributeTargets.Field)]
public class TableInfoAttribute(string name, SchemaName schemaName = SchemaName.Public, string? primaryKey = null) : Attribute
{
	public string Name { get; } = name;
	public SchemaName Schema { get; } = schemaName;
	public string? PrimaryKey { get; } = primaryKey;
}

public enum CtsTables
{
	[TableInfo("ct_addresses", SchemaName.Cts, "adr_id")]
	CtAddresses,
	[TableInfo("ct_alloc_routines", SchemaName.Cts, "rou_id")]
	CtAllocRoutines,
	[TableInfo("ct_animal_changes", SchemaName.Cts, "ach_id")]
	CtAnimalChanges,
	[TableInfo("ct_animal_claims", SchemaName.Cts, "anc_id")]
	CtAnimalClaims,
	[TableInfo("ct_animal_corr_summ_errors", SchemaName.Cts, "ase_id")]
	CtAnimalCorrSummErrors,
	[TableInfo("ct_animal_correct_summaries", SchemaName.Cts, "acs_id")]
	CtAnimalCorrectSummaries,
	[TableInfo("ct_animal_identifiers", SchemaName.Cts, "aid_id")]
	CtAnimalIdentifiers,
	[TableInfo("ct_animal_relationships", SchemaName.Cts, "aar_id")]
	CtAnimalRelationships,
	[TableInfo("ct_animal_statuses", SchemaName.Cts, "ast_id")]
	CtAnimalStatuses,
	[TableInfo("ct_applic_statuses", SchemaName.Cts, "aps_id")]
	CtApplicStatuses,
	[TableInfo("ct_application_late_days", SchemaName.Cts, "ald_id")]
	CtApplicationLateDays,
	[TableInfo("ct_batch_retention_conf", SchemaName.Cts, "id")]
	CtBatchRetentionConf,
	[TableInfo("ct_breeds", SchemaName.Cts, "brd_id")]
	CtBreeds,
	[TableInfo("ct_cla_extract", SchemaName.Cts, "cle_id")]
	CtClaExtract,
	[TableInfo("ct_cla_extract_detail", SchemaName.Cts, "cld_id")]
	CtClaExtractDetail,
	[TableInfo("ct_cla_extract_dm", SchemaName.Cts, "cle_id")]
	CtClaExtractDm,
	[TableInfo("ct_cla_mini_detail", SchemaName.Cts, "cld_id")]
	CtClaMiniDetail,
	[TableInfo("ct_cla_mini_extract", SchemaName.Cts, "cle_id")]
	CtClaMiniExtract,
	[TableInfo("ct_claim_statuses", SchemaName.Cts, "cls_id")]
	CtClaimStatuses,
	[TableInfo("ct_claim_types", SchemaName.Cts, "clt_id")]
	CtClaimTypes,
	[TableInfo("ct_cm_authorities", SchemaName.Cts, "cma_id")]
	CtCmAuthorities,
	[TableInfo("ct_cm_measures_results", SchemaName.Cts, "cmr_id")]
	CtCmMeasuresResults,
	[TableInfo("ct_comms_addresses", SchemaName.Cts, "coa_id")]
	CtCommsAddresses,
	[TableInfo("ct_cond_variant_groupings", SchemaName.Cts, "cvg_id")]
	CtCondVariantGroupings,
	[TableInfo("ct_condition_activities", SchemaName.Cts, "cac_id")]
	CtConditionActivities,
	[TableInfo("ct_condition_marker_errors", SchemaName.Cts, "cme_id")]
	CtConditionMarkerErrors,
	[TableInfo("ct_condition_markers", SchemaName.Cts, "com_id")]
	CtConditionMarkers,
	[TableInfo("ct_condition_types", SchemaName.Cts, "cot_id")]
	CtConditionTypes,
	[TableInfo("ct_condition_variants", SchemaName.Cts, "cov_id")]
	CtConditionVariants,
	[TableInfo("ct_conditions", SchemaName.Cts, "con_id")]
	CtConditions,
	[TableInfo("ct_counties", SchemaName.Cts, "cty_id")]
	CtCounties,
	[TableInfo("ct_counties_migration", SchemaName.Cts, "cty_id")]
	CtCountiesMigration,
	[TableInfo("ct_countries", SchemaName.Cts, "cry_id")]
	CtCountries,
	[TableInfo("ct_cps167_report", SchemaName.Cts, "kns_id")]
	CtCps167Report,
	[TableInfo("ct_cts164_handshake_file_keys", SchemaName.Cts, "id")]
	CtCts164HandshakeFileKeys,
	[TableInfo("ct_cts_users", SchemaName.Cts, "cus_id")]
	CtCtsUsers,
	[TableInfo("ct_eartag_formats", SchemaName.Cts, "etf_id")]
	CtEartagFormats,
	[TableInfo("ct_eartag_reason_flags", SchemaName.Cts, "erf_id")]
	CtEartagReasonFlags,
	[TableInfo("ct_eartag_reasons", SchemaName.Cts, "etr_id")]
	CtEartagReasons,
	[TableInfo("ct_eartag_staging", SchemaName.Cts, "est_id")]
	CtEartagStaging,
	[TableInfo("ct_eartag_types", SchemaName.Cts, "ett_id")]
	CtEartagTypes,
	[TableInfo("ct_eartags", SchemaName.Cts, "etg_id")]
	CtEartags,
	[TableInfo("ct_electronic_identifiers", SchemaName.Cts, "eid_id")]
	CtElectronicIdentifiers,
	[TableInfo("ct_email_log", SchemaName.Cts, "eml_id")]
	CtEmailLog,
	[TableInfo("ct_ereport_files", SchemaName.Cts, "ere_id")]
	CtEreportFiles,
	[TableInfo("ct_ereport_load_messages", SchemaName.Cts, "id")]
	CtEreportLoadMessages,
	[TableInfo("ct_ereport_locks", SchemaName.Cts, "id")]
	CtEreportLocks,
	[TableInfo("ct_ereport_process_messages", SchemaName.Cts, "id")]
	CtEreportProcessMessages,
	[TableInfo("ct_ext_cetd_eartag", SchemaName.Cts, "id")]
	CtExtCetdEartag,
	[TableInfo("ct_ext_ni_district", SchemaName.Cts, "id")]
	CtExtNiDistrict,
	[TableInfo("ct_ext_special_herd", SchemaName.Cts, "id")]
	CtExtSpecialHerd,
	[TableInfo("ct_file_layouts", SchemaName.Cts, "id")]
	CtFileLayouts,
	[TableInfo("ct_hsf_sequences", SchemaName.Cts, "id")]
	CtHsfSequences,
	[TableInfo("ct_insert_update_log", SchemaName.Cts, "iul_id")]
	CtInsertUpdateLog,
	[TableInfo("ct_issued_documents", SchemaName.Cts, "ido_id")]
	CtIssuedDocuments,
	[TableInfo("ct_issuing_authorities", SchemaName.Cts, "isa_id")]
	CtIssuingAuthorities,
	[TableInfo("ct_label_requests", SchemaName.Cts, "lar_id")]
	CtLabelRequests,
	[TableInfo("ct_label_summaries", SchemaName.Cts, "las_id")]
	CtLabelSummaries,
	[TableInfo("ct_late_days", SchemaName.Cts, "lda_id")]
	CtLateDays,
	[TableInfo("ct_letters", SchemaName.Cts, "let_id")]
	CtLetters,
	[TableInfo("ct_loc_type_rel_combs", SchemaName.Cts, "lrc_id")]
	CtLocTypeRelCombs,
	[TableInfo("ct_location_id_formats", SchemaName.Cts, "lif_id")]
	CtLocationIdFormats,
	[TableInfo("ct_location_identifiers", SchemaName.Cts, "lid_id")]
	CtLocationIdentifiers,
	[TableInfo("ct_location_party_rel_types", SchemaName.Cts, "lpt_id")]
	CtLocationPartyRelTypes,
	[TableInfo("ct_location_party_rels", SchemaName.Cts, "lpr_id")]
	CtLocationPartyRels,
	[TableInfo("ct_location_rel_types", SchemaName.Cts, "lrt_id")]
	CtLocationRelTypes,
	[TableInfo("ct_location_relationships", SchemaName.Cts, "llr_id")]
	CtLocationRelationships,
	[TableInfo("ct_location_types", SchemaName.Cts, "lty_id")]
	CtLocationTypes,
	[TableInfo("ct_locations", SchemaName.Cts, "loc_id")]
	CtLocations,
	[TableInfo("ct_locations_faker", SchemaName.Cts, "id")]
	CtLocationsFaker,
	[TableInfo("ct_locrestrictionstoanimals", SchemaName.Cts, "id")]
	CtLocrestrictionstoanimals,
	[TableInfo("ct_mgt_control_errors", SchemaName.Cts, "mce_id")]
	CtMgtControlErrors,
	[TableInfo("ct_mgt_wg_allocation_rules", SchemaName.Cts, "war_id")]
	CtMgtWgAllocationRules,
	[TableInfo("ct_mhs_to_cph", SchemaName.Cts, "id")]
	CtMhsToCph,
	[TableInfo("ct_mov_hst", SchemaName.Cts, "id")]
	CtMovHst,
	[TableInfo("ct_movt_corr_summ_errors", SchemaName.Cts, "mse_id")]
	CtMovtCorrSummErrors,
	[TableInfo("ct_movt_correct_summaries", SchemaName.Cts, "mcs_id")]
	CtMovtCorrectSummaries,
	[TableInfo("ct_msgtxt", SchemaName.Cts, "id")]
	CtMsgtxt,
	[TableInfo("ct_non_working_days", SchemaName.Cts, "nwd_id")]
	CtNonWorkingDays,
	[TableInfo("ct_param_group", SchemaName.Cts, "pgp_id")]
	CtParamGroup,
	[TableInfo("ct_param_header", SchemaName.Cts, "phd_id")]
	CtParamHeader,
	[TableInfo("ct_param_value", SchemaName.Cts, "pvl_id")]
	CtParamValue,
	[TableInfo("ct_param_value_group", SchemaName.Cts, "pvg_id")]
	CtParamValueGroup,
	[TableInfo("ct_parties", SchemaName.Cts, "par_id")]
	CtParties,
	[TableInfo("ct_parties_faker", SchemaName.Cts, "id")]
	CtPartiesFaker,
	[TableInfo("ct_ppaf_groupings", SchemaName.Cts, "ppg_id")]
	CtPpafGroupings,
	[TableInfo("ct_preprinted_appn_forms", SchemaName.Cts, "paf_id")]
	CtPreprintedAppnForms,
	[TableInfo("ct_probity_checks", SchemaName.Cts, "pch_id")]
	CtProbityChecks,
	[TableInfo("ct_ps9999_ahdb_data", SchemaName.Cts, "ran_id")]
	CtPs9999AhdbData,
	[TableInfo("ct_ps9999_ahdb_mov_history", SchemaName.Cts, "id")]
	CtPs9999AhdbMovHistory,
	[TableInfo("ct_recd_application_errors", SchemaName.Cts, "rae_id")]
	CtRecdApplicationErrors,
	[TableInfo("ct_recd_movement_errors", SchemaName.Cts, "rme_id")]
	CtRecdMovementErrors,
	[TableInfo("ct_received_applications", SchemaName.Cts, "rap_id")]
	CtReceivedApplications,
	[TableInfo("ct_received_movements", SchemaName.Cts, "rmo_id")]
	CtReceivedMovements,
	[TableInfo("ct_registered_animals", SchemaName.Cts, "ran_id")]
	CtRegisteredAnimals,
	[TableInfo("ct_registered_movements", SchemaName.Cts, "mov_id")]
	CtRegisteredMovements,
	[TableInfo("ct_reset_to_extract", SchemaName.Cts, "rte_id")]
	CtResetToExtract,
	[TableInfo("ct_sbcs_ext", SchemaName.Cts, "id")]
	CtSbcsExt,
	[TableInfo("ct_schemes", SchemaName.Cts, "sch_id")]
	CtSchemes,
	[TableInfo("ct_stage_files", SchemaName.Cts, "stf_id")]
	CtStageFiles,
	[TableInfo("ct_stage_locks", SchemaName.Cts, "id")]
	CtStageLocks,
	[TableInfo("ct_stage_messages", SchemaName.Cts, "id")]
	CtStageMessages,
	[TableInfo("ct_sublocation_types", SchemaName.Cts, "slt_id")]
	CtSublocationTypes,
	[TableInfo("ct_susp_animal_errors", SchemaName.Cts, "sae_id")]
	CtSuspAnimalErrors,
	[TableInfo("ct_susp_cm_measure_results", SchemaName.Cts, "smr_id")]
	CtSuspCmMeasureResults,
	[TableInfo("ct_susp_condition_markers", SchemaName.Cts, "scm_id")]
	CtSuspConditionMarkers,
	[TableInfo("ct_susp_movement_errors", SchemaName.Cts, "sme_id")]
	CtSuspMovementErrors,
	[TableInfo("ct_suspended_animals", SchemaName.Cts, "san_id")]
	CtSuspendedAnimals,
	[TableInfo("ct_suspended_movements", SchemaName.Cts, "smo_id")]
	CtSuspendedMovements,
	[TableInfo("ct_suspense_char_alloc_rules", SchemaName.Cts, "sca_id")]
	CtSuspenseCharAllocRules,
	[TableInfo("ct_suspense_wg_alloc_rules", SchemaName.Cts, "swa_id")]
	CtSuspenseWgAllocRules,
	[TableInfo("ct_valid_applications", SchemaName.Cts, "vap_id")]
	CtValidApplications,
	[TableInfo("ct_web_users", SchemaName.Cts, "wur_id")]
	CtWebUsers,
	[TableInfo("ct_wg_autoallocations", SchemaName.Cts, "wga_id")]
	CtWgAutoallocations,
	[TableInfo("ct_wg_super_assignments", SchemaName.Cts, "wsa_id")]
	CtWgSuperAssignments,
	[TableInfo("ct_wg_user_assignments", SchemaName.Cts, "wua_id")]
	CtWgUserAssignments,
	[TableInfo("ct_workgroups", SchemaName.Cts, "wgp_id")]
	CtWorkgroups
}
