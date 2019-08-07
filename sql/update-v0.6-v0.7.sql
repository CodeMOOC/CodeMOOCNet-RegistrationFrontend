-- Add password reset column
ALTER TABLE `Registrations`
  ADD COLUMN `PasswordResetSecret` CHAR(10) DEFAULT NULL AFTER `ConfirmationTimestamp`;
