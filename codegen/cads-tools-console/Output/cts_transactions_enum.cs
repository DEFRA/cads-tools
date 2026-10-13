// Auto-generated code from PostgreSQL schema
using System.ComponentModel;

public enum CtsTransactionTables
{
	[TableInfo("ct_addresses", SchemaName.CtsTransactions, "trans_id")]
	CtAddresses,
	[TableInfo("ct_animal_changes", SchemaName.CtsTransactions, "trans_id")]
	CtAnimalChanges,
	[TableInfo("ct_animal_claims", SchemaName.CtsTransactions, "trans_id")]
	CtAnimalClaims,
	[TableInfo("ct_animal_corr_summ_errors", SchemaName.CtsTransactions, "trans_id")]
	CtAnimalCorrSummErrors,
	[TableInfo("ct_animal_correct_summaries", SchemaName.CtsTransactions, "trans_id")]
	CtAnimalCorrectSummaries,
	[TableInfo("ct_animal_identifiers", SchemaName.CtsTransactions, "trans_id")]
	CtAnimalIdentifiers,
	[TableInfo("ct_animal_relationships", SchemaName.CtsTransactions, "trans_id")]
	CtAnimalRelationships,
	[TableInfo("ct_animal_statuses", SchemaName.CtsTransactions, "trans_id")]
	CtAnimalStatuses,
	[TableInfo("ct_applic_statuses", SchemaName.CtsTransactions, "trans_id")]
	CtApplicStatuses,
	[TableInfo("ct_application_late_days", SchemaName.CtsTransactions, "trans_id")]
	CtApplicationLateDays,
	[TableInfo("ct_cla_extract", SchemaName.CtsTransactions, "trans_id")]
	CtClaExtract,
	[TableInfo("ct_cla_extract_detail", SchemaName.CtsTransactions, "trans_id")]
	CtClaExtractDetail,
	[TableInfo("ct_cla_extract_dm", SchemaName.CtsTransactions, "trans_id")]
	CtClaExtractDm,
	[TableInfo("ct_cla_mini_detail", SchemaName.CtsTransactions, "trans_id")]
	CtClaMiniDetail,
	[TableInfo("ct_cla_mini_extract", SchemaName.CtsTransactions, "trans_id")]
	CtClaMiniExtract,
	[TableInfo("ct_cm_measures_results", SchemaName.CtsTransactions, "trans_id")]
	CtCmMeasuresResults,
	[TableInfo("ct_comms_addresses", SchemaName.CtsTransactions, "trans_id")]
	CtCommsAddresses,
	[TableInfo("ct_condition_marker_errors", SchemaName.CtsTransactions, "trans_id")]
	CtConditionMarkerErrors,
	[TableInfo("ct_condition_markers", SchemaName.CtsTransactions, "trans_id")]
	CtConditionMarkers,
	[TableInfo("ct_cps167_report", SchemaName.CtsTransactions, "trans_id")]
	CtCps167Report,
	[TableInfo("ct_cts_users", SchemaName.CtsTransactions, "trans_id")]
	CtCtsUsers,
	[TableInfo("ct_eartag_staging", SchemaName.CtsTransactions, "trans_id")]
	CtEartagStaging,
	[TableInfo("ct_eartags", SchemaName.CtsTransactions, "trans_id")]
	CtEartags,
	[TableInfo("ct_electronic_identifiers", SchemaName.CtsTransactions, "trans_id")]
	CtElectronicIdentifiers,
	[TableInfo("ct_email_log", SchemaName.CtsTransactions, "trans_id")]
	CtEmailLog,
	[TableInfo("ct_ereport_files", SchemaName.CtsTransactions, "trans_id")]
	CtEreportFiles,
	[TableInfo("ct_ereport_load_messages", SchemaName.CtsTransactions, "trans_id")]
	CtEreportLoadMessages,
	[TableInfo("ct_ereport_locks", SchemaName.CtsTransactions, "trans_id")]
	CtEreportLocks,
	[TableInfo("ct_ereport_process_messages", SchemaName.CtsTransactions, "trans_id")]
	CtEreportProcessMessages,
	[TableInfo("ct_ext_cetd_eartag", SchemaName.CtsTransactions, "trans_id")]
	CtExtCetdEartag,
	[TableInfo("ct_insert_update_log", SchemaName.CtsTransactions, "trans_id")]
	CtInsertUpdateLog,
	[TableInfo("ct_issued_documents", SchemaName.CtsTransactions, "trans_id")]
	CtIssuedDocuments,
	[TableInfo("ct_label_requests", SchemaName.CtsTransactions, "trans_id")]
	CtLabelRequests,
	[TableInfo("ct_label_summaries", SchemaName.CtsTransactions, "trans_id")]
	CtLabelSummaries,
	[TableInfo("ct_letters", SchemaName.CtsTransactions, "trans_id")]
	CtLetters,
	[TableInfo("ct_location_identifiers", SchemaName.CtsTransactions, "trans_id")]
	CtLocationIdentifiers,
	[TableInfo("ct_location_party_rels", SchemaName.CtsTransactions, "trans_id")]
	CtLocationPartyRels,
	[TableInfo("ct_location_relationships", SchemaName.CtsTransactions, "trans_id")]
	CtLocationRelationships,
	[TableInfo("ct_locations", SchemaName.CtsTransactions, "trans_id")]
	CtLocations,
	[TableInfo("ct_locations_faker", SchemaName.CtsTransactions, "trans_id")]
	CtLocationsFaker,
	[TableInfo("ct_locrestrictionstoanimals", SchemaName.CtsTransactions, "trans_id")]
	CtLocrestrictionstoanimals,
	[TableInfo("ct_mgt_control_errors", SchemaName.CtsTransactions, "trans_id")]
	CtMgtControlErrors,
	[TableInfo("ct_mhs_to_cph", SchemaName.CtsTransactions, "trans_id")]
	CtMhsToCph,
	[TableInfo("ct_mov_hst", SchemaName.CtsTransactions, "trans_id")]
	CtMovHst,
	[TableInfo("ct_movt_corr_summ_errors", SchemaName.CtsTransactions, "trans_id")]
	CtMovtCorrSummErrors,
	[TableInfo("ct_movt_correct_summaries", SchemaName.CtsTransactions, "trans_id")]
	CtMovtCorrectSummaries,
	[TableInfo("ct_parties", SchemaName.CtsTransactions, "trans_id")]
	CtParties,
	[TableInfo("ct_parties_faker", SchemaName.CtsTransactions, "trans_id")]
	CtPartiesFaker,
	[TableInfo("ct_ppaf_groupings", SchemaName.CtsTransactions, "trans_id")]
	CtPpafGroupings,
	[TableInfo("ct_preprinted_appn_forms", SchemaName.CtsTransactions, "trans_id")]
	CtPreprintedAppnForms,
	[TableInfo("ct_ps9999_ahdb_data", SchemaName.CtsTransactions, "trans_id")]
	CtPs9999AhdbData,
	[TableInfo("ct_ps9999_ahdb_mov_history", SchemaName.CtsTransactions, "trans_id")]
	CtPs9999AhdbMovHistory,
	[TableInfo("ct_recd_application_errors", SchemaName.CtsTransactions, "trans_id")]
	CtRecdApplicationErrors,
	[TableInfo("ct_recd_movement_errors", SchemaName.CtsTransactions, "trans_id")]
	CtRecdMovementErrors,
	[TableInfo("ct_received_applications", SchemaName.CtsTransactions, "trans_id")]
	CtReceivedApplications,
	[TableInfo("ct_received_movements", SchemaName.CtsTransactions, "trans_id")]
	CtReceivedMovements,
	[TableInfo("ct_registered_animals", SchemaName.CtsTransactions, "trans_id")]
	CtRegisteredAnimals,
	[TableInfo("ct_registered_movements", SchemaName.CtsTransactions, "trans_id")]
	CtRegisteredMovements,
	[TableInfo("ct_reset_to_extract", SchemaName.CtsTransactions, "trans_id")]
	CtResetToExtract,
	[TableInfo("ct_sbcs_ext", SchemaName.CtsTransactions, "trans_id")]
	CtSbcsExt,
	[TableInfo("ct_stage_files", SchemaName.CtsTransactions, "trans_id")]
	CtStageFiles,
	[TableInfo("ct_stage_locks", SchemaName.CtsTransactions, "trans_id")]
	CtStageLocks,
	[TableInfo("ct_stage_messages", SchemaName.CtsTransactions, "trans_id")]
	CtStageMessages,
	[TableInfo("ct_susp_animal_errors", SchemaName.CtsTransactions, "trans_id")]
	CtSuspAnimalErrors,
	[TableInfo("ct_susp_cm_measure_results", SchemaName.CtsTransactions, "trans_id")]
	CtSuspCmMeasureResults,
	[TableInfo("ct_susp_condition_markers", SchemaName.CtsTransactions, "trans_id")]
	CtSuspConditionMarkers,
	[TableInfo("ct_susp_movement_errors", SchemaName.CtsTransactions, "trans_id")]
	CtSuspMovementErrors,
	[TableInfo("ct_suspended_animals", SchemaName.CtsTransactions, "trans_id")]
	CtSuspendedAnimals,
	[TableInfo("ct_suspended_movements", SchemaName.CtsTransactions, "trans_id")]
	CtSuspendedMovements,
	[TableInfo("ct_valid_applications", SchemaName.CtsTransactions, "trans_id")]
	CtValidApplications,
	[TableInfo("ct_web_users", SchemaName.CtsTransactions, "trans_id")]
	CtWebUsers,
	[TableInfo("ct_wg_autoallocations", SchemaName.CtsTransactions, "trans_id")]
	CtWgAutoallocations,
	[TableInfo("ct_wg_super_assignments", SchemaName.CtsTransactions, "trans_id")]
	CtWgSuperAssignments,
	[TableInfo("ct_wg_user_assignments", SchemaName.CtsTransactions, "trans_id")]
	CtWgUserAssignments,
	[TableInfo("ct_workgroups", SchemaName.CtsTransactions, "trans_id")]
	CtWorkgroups
}
