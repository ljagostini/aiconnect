namespace Percolore.Core.AccessControl
{
    public class PermissionBuilder
    {
        Profile _profile;

        public PermissionBuilder(Profile profile)
        {
            _profile = profile;
        }

        public Permissions Generate()
        {
            Permissions permissions = new Permissions();

            switch (_profile)
            {
                case Profile.Administrador:
                    {
                        permissions.HabilitarDispensaSequencial = true;
                        permissions.HabilitarFormulaPersonalizada = true;
                        permissions.HabilitarModoSimulacao = true;
                        permissions.HabilitarTesteRecipiente = true;
                        permissions.HabilitarControleNivelColorante = true;
                        permissions.HabilitarCircuitoColorante = true;
                        permissions.EditarInformacoesAbaUnidadeMedida = true;
                        permissions.EditarInformacoesAbaInicializacaoCircuitos = true;
                        permissions.EditarInformacoesAbaPurga = true;
                        permissions.EditarInformacoesAbaSenha = true;

                        break;
                    }
                case Profile.Tecnico:
                    {
                        permissions.HabilitarDispensaSequencial = false;
                        permissions.HabilitarFormulaPersonalizada = false;
                        permissions.HabilitarModoSimulacao = true;
                        permissions.HabilitarTesteRecipiente = false;
                        permissions.HabilitarControleNivelColorante = false;
                        permissions.HabilitarCircuitoColorante = false;
                        permissions.EditarInformacoesAbaUnidadeMedida = false;
                        permissions.EditarInformacoesAbaInicializacaoCircuitos = false;
                        permissions.EditarInformacoesAbaPurga = false;
                        permissions.EditarInformacoesAbaSenha = false;

                        break;
                    }
                default:
                    break;
            }

            return permissions;
        }

    }
}